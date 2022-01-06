using Newtonsoft.Json;

namespace junglestate {
    class Program {
        private readonly HttpClient client;
        private readonly Tuple<string?, string?> configs;
        private readonly BaseMonkey monkey;

        private Program(BaseMonkey monkey, Tuple<string?, string?> configs) {
            this.configs = configs;
            this.monkey = monkey;
            this.client = new HttpClient();
        }

        private async Task joinGame() {
            // non-user configs
            int SLEEP_TIME = 50;

            // join game (fetch request)
            var response = await client.PostAsync(configs.Item1+"joinGame/client/"+configs.Item2, new StringContent(""));
            var json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);

            // check if joining was successful
            if (data != null){
                if (data.ok == false) {
                    Console.WriteLine("Connection to game failed!");
                    Console.WriteLine(data.msg);
                }
                else {
                    int score = 0;
                    // loop with regular request for the field followed by an instruction for the server what to do
                    while (true) {
                        Tuple<bool, int> returnedData = await getData(monkey, configs);
                        Thread.Sleep(SLEEP_TIME);
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
        private async Task<Tuple<bool, int>> getData(BaseMonkey monkey, Tuple<string?, string?> configs) {
            // get the map
            var stringTask = client.GetStringAsync(configs.Item1+"view");
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);

            bool gameOver = false;
            int score = 0;

            if (data != null) {
                score = data.points.ToObject<int>(); //, System.Globalization.NumberStyles.Integer
                // ask user written algorithm about what to do
                if (data.lives > 0) {
                    Console.WriteLine("");
                    Console.WriteLine("Round: "+data.round);
                    Console.WriteLine("Health: "+data.lives);
                    Console.WriteLine("Ammo: "+data.coconuts);
                    Console.WriteLine("Score: "+data.points);
                    playerBehaviour(monkey, configs, data.field, data.lives.ToObject<int>(), data.coconuts.ToObject<int>(), data.points.ToObject<int>(), data.round.ToObject<int>());
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

        private void move(Tuple<string, string> configs, int direction) {
            // option for user written algorithm
            // direction defines the direction the player moves
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand(configs, 1, direction);
            }
            // if an invalid direction is given or -1 is stated the character does nothing
            else {
                sendCommand(configs, 0, -1);
            }
        }

        private void attack(Tuple<string, string> configs, int direction) {
            // option for user written algorithm
            // direction defines the direction the player shoots
            if (direction >= 0 && direction <= 7) {
                sendCommand(configs, 2, direction);
            }
            // if an invalid direction is given the character does nothing
            else {
                sendCommand(configs, 0, -1);
            }
        }

        private async void sendCommand(Tuple<string, string> configs, int type, int direction) {
            // send the chosen action to the server
            try {
                var response = await client.PostAsync(configs.Item1+"action/"+type+"/"+direction, new StringContent(""));
                
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

        private static Tuple<string, string, bool> loadConfigs(Monkey monkey) {
            // configurations

            // url of server
            // e.g. http://localhost:5500/
            string url = "http://localhost:5500/";

            // name of player
            // e.g. Max Mustermann
            string name = monkey.name;

            // debug mode
            // app mode requires you to enter url and name after starting app
            bool appMode = true;

            Tuple<string, string, bool> configs = new Tuple<string, string, bool>(url, name, appMode);
            return configs;
        }

        private void playerBehaviour(BaseMonkey monkey, Tuple<string, string> c, string[] field, int health, int ammo, int score, int round) {   
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
        // Tuple<string, string> c: configurations, can be ignored but must be given as an argument for attack() and move()

            Move next = monkey.nextMove(new GameState(getCells(field), round, new PlayerInfo(monkey.name, health, ammo, score)));
            switch(next.action) {
                case Action.MOVE:
                    move(c, (int)next.direction);
                    break;
                case Action.THROW:
                    attack(c, (int)next.direction);
                    break;
                default:
                    move(c, (int)Action.STAY);
                    break;
            }
        }

        private static Cell[][] getCells(string[] field) {
            Cell[][] cells = new Cell[5][];
            int rowIndex = 0;
            foreach (string row in field) {
                cells[rowIndex] = new Cell[5];
                for (int i = 0; i < row.Length; i++) {
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

        public static async Task ProgramMain(string[] args, Monkey monkey) {
            // start the process
            Tuple<string, string, bool> rawConfigs = loadConfigs(monkey);
            Tuple<string?, string?> configs;

            if (rawConfigs.Item3 == false) {
                // normal start
                configs = new Tuple<string?, string?>(rawConfigs.Item1, rawConfigs.Item2);
            }
            else {
                // app mode start
                try {
                    // ask url
                    Console.WriteLine("Enter url of server");
                    Console.WriteLine("e.g. http://localhost:5500/");
                    string? url = Console.ReadLine();

                    // ask name
                    Console.Clear();
                    Console.WriteLine("Enter name of player");
                    string? name = Console.ReadLine();

                    // start
                    Console.Clear();
                    configs = new Tuple<string?, string?>(url, name);
                }
                catch {
                    Console.WriteLine("Invalid input.");
                    return;
                }
            }
            Console.Clear();
            Program program = new Program(monkey, configs);
            await program.joinGame();
        }
    }
}