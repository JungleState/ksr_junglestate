using CommandLine;

// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Download Json.NET: "dotnet add package Newtonsoft.Json
//     - Run: "dotnet run"

namespace junglestate {
    public class Monkey : BaseMonkey {
        public Monkey(string name) : base(name) {}
        public override Move nextMove(GameState state) {
            Direction direction = selectRandomDirection(state);
            return new Move(Action.MOVE, direction, state.round);
        }

        private Direction selectRandomDirection(GameState state) {
            List<Direction> freeDirs = computeFreeDirections(state);
            Random random = new System.Random();
            Random r = new Random();
            return freeDirs[r.Next(freeDirs.Count)];
        }

        private List<Direction> computeFreeDirections(GameState state) {
            List<Direction> result = new List<Direction>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                if (state.getCell(dir).isMoveable()) {
                    result.Add(dir);
                }
            }
            return result;
        }


        public class Options {
            [Option('s', "server", Required = false, HelpText = "Select server URL.", Default = "http://localhost:5500/")]
            public string Server { get; set; }
            [Option('n', "name", Required = false, HelpText = "Select name.", Default = "Huey")]
            public string Name { get; set; }
            [Option('g', "game", Required = false, HelpText = "Select game (none to create a new game).", Default = "")]
            public string GameId { get; set; }
            [Option('p', "password", Required = false, HelpText = "The password.", Default = "")]
            public string Password { get; set; }
            //string server = "http://localhost:5500/", string name = "Huey", string gameId = "", string password = ""
        }
        public static async Task Main(string[] args) {
            JungleConfig config = new JungleConfig();
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync<Options>(o =>
                   {
                        config.serverAddress = new Uri(o.Server);
                        config.gameId = o.GameId;
                        config.password = o.Password;
                        Monkey monkey = new Monkey(o.Name);
                        return Program.ProgramMain(config, monkey);
                   });
        }
    }
}