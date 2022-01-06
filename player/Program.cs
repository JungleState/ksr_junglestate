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
                score = data.points.ToObject<int>(); //, System.Globalization.NumberStyles.Integer
                // ask user written algorithm about what to do
                if (data.lives > 0) {
                    Console.WriteLine("");
                    Console.WriteLine("Field: "+data.field);
                    Console.WriteLine("Round: "+data.round);
                    Console.WriteLine("Health: "+data.lives);
                    Console.WriteLine("Ammo: "+data.coconuts);
                    Console.WriteLine("Score: "+data.points);
                    playerBehaviour(data.field.ToObject<string[]>(), data.lives.ToObject<int>(), data.coconuts.ToObject<int>(), data.points.ToObject<int>(), data.round.ToObject<int>());
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

        private void move(int direction) {
            // option for user written algorithm
            // direction defines the direction the player moves
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand(1, direction);
            }
            // if an invalid direction is given or -1 is stated the character does nothing
            else {
                sendCommand(0, -1);
            }
        }

        private void attack(int direction) {
            // option for user written algorithm
            // direction defines the direction the player shoots
            if (direction >= 0 && direction <= 7) {
                sendCommand(2, direction);
            }
            // if an invalid direction is given the character does nothing
            else {
                sendCommand(0, -1);
            }
        }

        private async void sendCommand(int type, int direction) {
            // send the chosen action to the server
            try {
                var response = await client.PostAsync(new Uri(config.serverAddress, "action/"+type+"/"+direction), new StringContent(""));
                
                string[] actionsArray = new string[] {"stay", "move", "attack"};
                string[] directionsArray = new string[] {"up", "up right", "right", "down right", "down", "down left", "left", "up left", "on the spot"};
                if (direction < 0) {
                    direction = 8;
                }
                Console.WriteLine("-> Action: "+actionsArray[type]+" "+directionsArray[direction]);
            }
            catch {
                // Console.WriteLine("");
                // Console.WriteLine("Command could not be sent");
            }
        }

        private void playerBehaviour(string[] field, int health, int ammo, int score, int round) {   
        // get user written algorithm
        // attack:  attack(c, DIRECTION)   options for DIRECTION: [0, 7]
        // move:    move(c, DIRECTION)     options for DIRECTION: -1, 0, 2, 4, 6
        // DIRECTION: integer
        //    -1: No direction
        //    0: up
        //    1: up right
        //    2: right
        //    3: down right
        //    4: down
        //    5: down left
        //    6: left
        //    7: up left

        // "  ": plain:     empty
        // "FF": jungle:    wall
        // "CC": coconut:   +1 ammo
        // "BB": banana:    +1 health
        // "PP": pineapple: +1 score

        // string field: 5x5 matrix of surrounding
        // int health: health of character
        // int ammo: amount of ammo left
        // int score: score of player
        // int round: round of the game
            Move next = monkey.nextMove(new GameState(getCells(field), round, new PlayerInfo(monkey.name, health, ammo, score)));
            switch(next.action) {
                case Action.MOVE:
                    move((int)next.direction);
                    break;
                case Action.THROW:
                    attack((int)next.direction);
                    break;
                default:
                    move((int)Action.STAY);
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