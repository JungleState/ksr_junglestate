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

        enum Mode{
            Farm,
            Hunt,
            Flee
        }
        public override Move nextMove(GameState state) {
            Direction direction = selectDirection(state, Mode.Farm);
            return new Move(Action.MOVE, direction, state.round);
        }

        private Direction selectDirection(GameState state, Mode mode) {
            List<Direction> freeDirs = computeFreeDirections(state);
            if(mode == Mode.Farm){ //Farm Mode
                if(!freeDirs.Contains(LastDirection)){ //! = quasi if not
                    Random random = new System.Random();
                    Random r = new Random();
                    Direction NewDirection = freeDirs[r.Next(freeDirs.Count)];

                    while(LastDirection.opposite() == NewDirection){ //so it doesn't go back the same direction

                        NewDirection=freeDirs[r.Next(freeDirs.Count)];
                    }

                    LastDirection=NewDirection;
                    return NewDirection;
                }
            }
            if(mode == Mode.Flee){
                return

            }
            if(mode == Mode.Hunt){

            }
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