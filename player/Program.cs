using Newtonsoft.Json;
using CommandLine;

namespace junglestate {
    public sealed class JungleConfig {
        public Uri serverAddress = new Uri("http://localhost:5500/");
        public string gameId = "";
        public string password = "";
        public int delay_ms = 500;
    }

    ///<summary>The main program for junglecamp monkey bots.</summary>
    ///<remarks>Once a program is started, it maintains a HTTP connection to the game server
    ///  listens for state updates and calls the monkey's <see cref="BaseMonkey.nextMove(GameState)"/>
    ///  method determine the monkey's behavior.
    ///</remarks>
    class Program {
        private readonly HttpClient client;
        private readonly JungleConfig config;
        private readonly BaseMonkey monkey;

        private Program(BaseMonkey monkey, JungleConfig config) {
            this.config = config;
            this.monkey = monkey;
            this.client = new HttpClient();
        }

        private async Task joinGame() {
            // join game (fetch request)
            var joinData = new {
                player_name = monkey.Name,
                player_mode = "client",
                password = config.password,
                mode = String.IsNullOrEmpty(config.gameId) ? "newGame" : "joinExisting",
                game_id = config.gameId
            };
            string jsonData = JsonConvert.SerializeObject(joinData);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri(config.serverAddress, "joinGame"), content);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) {
                Console.WriteLine(json);
                return;
            }
            dynamic? data = JsonConvert.DeserializeObject(json);

            // check if joining was successful
            if (data != null){
                if (data.ok == false) {
                    Console.WriteLine("Connection to game failed!");
                    Console.WriteLine((string) data.msg);
                }
                else {
                    int score = 0;
                    // loop with regular request for the field followed by an instruction for the server what to do
                    while (true) {
                        Tuple<bool, int> returnedData = await getData();
                        Thread.Sleep(config.delay_ms);
                        if (returnedData.Item1) {
                            score = returnedData.Item2;
                            break;
                        }
                    }
                    Console.WriteLine("");
                    Console.WriteLine("!!!!!!!!!!!");
                    Console.WriteLine("!GAME OVER!");
                    Console.WriteLine("!!!!!!!!!!!");
                    Console.WriteLine("Score: "+score);
                }   
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
        }
        private async Task<Tuple<bool, int>> getData() {
            // get the map
            var stringTask = client.GetStringAsync(new Uri(config.serverAddress, "view"));
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);

            bool gameOver = false;
            int score = 0;

            if (data != null) {
                GameState state = parseGameState(data);
                if (data.lives > 0) {
                    Console.WriteLine("Field: "+data.field);
                    Console.WriteLine("Round: "+data.round);
                    Console.WriteLine("Health: "+data.lives);
                    Console.WriteLine("Ammo: "+data.coconuts);
                    Console.WriteLine("Score: "+data.points);
                    playerBehaviour(state);
                }
                else {
                    // end loop from joinGame
                    gameOver = true;
                }
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
            return new Tuple<bool, int>(gameOver, score);
        }

        private GameState parseGameState(dynamic data) {
            return new GameState(getCells(data.field.ToObject<string[]>()),
                                 data.round.ToObject<int>(),
                                 new PlayerInfo(monkey.Name,
                                        data.lives.ToObject<int>(),
                                        data.coconuts.ToObject<int>(),
                                        data.points.ToObject<int>()));
        }

        private async void sendCommand(Action action, Direction direction) {
            // send the chosen action to the server
            try {
                Console.WriteLine("-> Action: "+action.ToString()+" "+direction.ToString());
                await client.PostAsync(new Uri(config.serverAddress, "action/"+(int)action+"/"+(int)direction), new StringContent(""));
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void playerBehaviour(GameState state) {   
            Move next = monkey.nextMove(state);
            switch(next.action) {
                case Action.MOVE:
                    if (next.direction.isMoveable()) {
                        sendCommand(next.action, next.direction);
                    }
                    break;
                case Action.THROW:
                    sendCommand(next.action, next.direction);
                    break;
                default:
                    sendCommand(Action.STAY, Direction.NONE);
                    break;
            }
        }

        private static Cell[][] getCells(string[] field) {
            Cell[][] cells = new Cell[5][];
            int rowIndex = 0;
            foreach (string row in field) {
                cells[rowIndex] = new Cell[5];
                for (int i = 0; i < row.Length / 2; i++) {
                    string cellString = row.Substring(i*2, 2);
                    cells[rowIndex][i] = getCell(cellString);
                }
                rowIndex++;
            }
            return cells;
        }

        private static Cell getCell(string cellString) {
            Item item = ItemInfo.fromCode(cellString);
            if (item == Item.PLAYER) {
                // TODO create player info from json response.
                PlayerInfo info = new PlayerInfo("player", 3, 2, 25);
                return Cell.PlayerCell(info);
            }
            return Cell.ItemCell(item);
        }

        [Verb("join", HelpText = "Join an existing game.")]
        class JoinOptions : GlobalOptions {
            [Option('g', "game", Required = true, HelpText = "Game id.", Default = "")]
            public string GameId { get; set; } = "";
            [Option('p', "password", Required = false, HelpText = "Game password.", Default = "")]
            public string Password { get; set; } = "";
        }
        [Verb("start", HelpText = "Start a new game.")]
        class StartOptions : GlobalOptions {
            //start options here
        }
        class GlobalOptions {
            [Option('s', "server", Required = false, HelpText = "Server URL.", Default = "http://localhost:5500/")]
            public string Server { get; set; } = "http://localhost:5500/";
            [Option('d', "delay", Required = false, HelpText = "Update delay in millis.", Default = 500)]
            public int Delay { get; set; } = 500;
            [Option('n', "name", Required = false, HelpText = "The monkey name, must be unique per server.", Default = "Hooey")]
            public string Name { get; set; } = "Hooey";
        }

        [Verb("ask", isDefault: true, HelpText = "Ask for options.")]
        class AskOptions : GlobalOptions {

        }
        public static async Task ProgramMain(string[] args, BaseMonkey monkey) {
            JungleConfig config = new JungleConfig();
            await Parser.Default.ParseArguments<JoinOptions, StartOptions, AskOptions>(args)
                    .MapResult(
                        (JoinOptions joinOpts) => JoinMain(joinOpts, monkey),
                        (StartOptions startOpts) => StartMain(startOpts, monkey),
                        (AskOptions askOpts) => AskMain(askOpts, monkey),
                        errs => Task.FromResult(1)
                    );
        }

        private static async Task JoinMain(JoinOptions options, BaseMonkey monkey) {
            JungleConfig config = new JungleConfig();
            config.serverAddress = new Uri(options.Server);
            config.gameId = options.GameId;
            config.password = options.Password;
            config.delay_ms = options.Delay;
            monkey.Name = options.Name;
            Program program = new Program(monkey, config);
            await program.joinGame();
        }

        private static async Task StartMain(StartOptions options, BaseMonkey monkey) {
            JungleConfig config = new JungleConfig();
            config.serverAddress = new Uri(options.Server);
            config.delay_ms = options.Delay;
            monkey.Name = options.Name;
            Program program = new Program(monkey, config);
            await program.joinGame();
        }

        private static async Task AskMain(AskOptions options, BaseMonkey monkey) {
            JungleConfig config = new JungleConfig();
            config.serverAddress = new Uri(options.Server);
            config.delay_ms = options.Delay;
            monkey.Name = options.Name;
            
            HttpClient client = new HttpClient();
            var stringTask = client.GetStringAsync(new Uri(config.serverAddress, "getGames"));
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);
            if (data == null) {
                throw new Exception("No games");
            }

            dynamic games = data.games;
            Console.WriteLine("Start your monkey as follows: ");
            Console.WriteLine("Start new game (0) (default)");
            int key = 0;
            foreach (dynamic game in games) {
                key++;
                Console.WriteLine($"Join game '{game.id}' ({key})");
            }
            string? input = Console.ReadLine();
            int selection;
            if (int.TryParse(input, out selection)) {
                config.gameId = games[selection - 1].id;
            }

            Program program = new Program(monkey, config);
            await program.joinGame();
        }

    }
}