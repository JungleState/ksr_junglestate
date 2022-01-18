// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Download Json.NET: "dotnet add package Newtonsoft.Json
//     - Run: "dotnet run"

namespace junglestate {
    ///<summary>A simple monkey that moves randomly in free directions.</summary>
    public class Monkey : BaseMonkey {
        private Direction LastDirection = Direction.RIGHT; //some default Direction
        public override Move nextMove(GameState state) {
            Direction direction = selectDirection(state);
            return new Move(Action.MOVE, direction, state.round);
        }

        private Direction selectDirection(GameState state) {
            List<Direction> freeDirs = computeFreeDirections(state);
            if (freeDirs.Contains(LastDirection)){
                return(LastDirection); //so the movement is in one straight line
            }
            Random random = new System.Random();
            Random r = new Random();
            return freeDirs[r.Next(freeDirs.Count)];
        }

        private List<Direction> computeFreeDirections(GameState state) {
            List<Direction> result = new List<Direction>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                if (dir.isMoveable() && state.getCell(dir).isFree()) {
                    result.Add(dir);
                }
            }
            return result;
        }
    }
}