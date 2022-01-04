using Program;
using System;

// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Download Json.NET: "dotnet add package Newtonsoft.Json
//     - Run: "dotnet run"

namespace junglestate {
    class Monkey : BaseMonkey {
        Monkey() {
            super("Max Mustermann");
        }
        public override Move nextMove(GameState state) {
            Direction direction = selectRandomDirection(state);
            return new Action{action = Action.MOVE, direction = direction, nextRound = state.round};
        }

        private Direction selectRandomDirection(GameState state) {
            List<Direction> freeDirs = computeFreeDirections(state);
            Random random = new System.Random();
            int r = random.NextInt(freeDirs.Size());
            return freeDirs[r];
        }

        private List<Direction> computeFreeDirs(GameState state) {
            List<Direction> results = new List<>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                Cell cell = dir.GetCell(state);
                if (cell.isMoveable()) {
                    result.Add(dir);
                }
            }
            return result;
        }

        static async Task Main(string[] args) {
            ProgramMain(args, new Monkey());
        }
    }
}