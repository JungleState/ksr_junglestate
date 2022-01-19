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
            Direction NewDirection;

            if(mode == Mode.Farm){ //Farm Mode
                Random random = new System.Random();
                Random r = new Random();
                if(!(freeDirs.Contains(LastDirection))){ //! = quasi if not

                    NewDirection = freeDirs[r.Next(freeDirs.Count)];
                    while(LastDirection.opposite() == NewDirection){ //so it doesn't go back the same direction

                        NewDirection=freeDirs[r.Next(freeDirs.Count)];
                    }
                    
                    LastDirection=NewDirection;
                }
                else{
                    NewDirection = LastDirection;
                }
                return NewDirection;
            }
            if(mode == Mode.Flee){
                return LastDirection;

            }
            if(mode == Mode.Hunt){
                return LastDirection;
            }
            else{
                return LastDirection;
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