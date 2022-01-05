﻿// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Download Json.NET: "dotnet add package Newtonsoft.Json
//     - Run: "dotnet run"

namespace junglestate {
    public class Monkey : BaseMonkey {
        public Monkey() : base("Max Mustermann") {
        }
        public override Move nextMove(GameState state) {
            Direction direction = selectRandomDirection(state);
            return new Move{action = Action.MOVE, direction = direction, nextRound = state.round};
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
                Cell cell = dir.GetCell(state);
                if (cell.isMoveable()) {
                    result.Add(dir);
                }
            }
            return result;
        }

        public static async Task Main(string[] args) {
            // starts the the program
            Program.ProgramMain(args, new Monkey());
        }
    }
}