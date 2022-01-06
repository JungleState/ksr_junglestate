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
                player_name = monkey.name,
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
                                 new PlayerInfo(monkey.name,
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

        public class Options {
            [Option('s', "server", Required = false, HelpText = "Server URL.", Default = "http://localhost:5500/")]
            public string Server { get; set; }
            [Option('g', "game", Required = false, HelpText = "Game id (none to create a new game).", Default = "")]
            public string GameId { get; set; }
            [Option('p', "password", Required = false, HelpText = "Game password.", Default = "")]
            public string Password { get; set; }
            [Option('d', "delay", Required = false, HelpText = "Update delay in millis.", Default = 500)]
            public int Delay { get; set; }
        }
        public static async Task ProgramMain(string[] args, BaseMonkey monkey) {
            JungleConfig config = new JungleConfig();
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync<Options>(o =>
                   {
                        config.serverAddress = new Uri(o.Server);
                        config.gameId = o.GameId;
                        config.password = o.Password;
                        config.delay_ms = o.Delay;
                        Program program = new Program(monkey, config);
                        return program.joinGame();
                   });
        }
    }
}