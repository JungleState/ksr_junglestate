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
            if(state.playerInfo.lives<=2){
                Direction direction = selectDirection(state, Mode.Flee);
                return new Move(Action.MOVE, direction, state.round, "RUN BOYYS RUN");
            }
            else{
                Direction direction = selectDirection(state, Mode.Farm);
                return new Move(Action.MOVE, direction, state.round, "UNCLE BEN STYLE");
            }
        }

        private Direction selectDirection(GameState state, Mode mode) {


            if(mode == Mode.Farm){ //Farm Mode

                foreach(Direction dir in Enum.GetValues(typeof(Direction))){
                    if (dir.isMoveable() && state.getCell(dir).item == Item.PINEAPPLE) {
                        return dir;
                    }
                    if (dir.isMoveable() && state.getCell(dir).item == Item.BANANA) {
                        return dir;
                    }
                    if (dir.isMoveable() && state.getCell(dir).item == Item.COCONUT && state.playerInfo.coconuts < 3) {
                        return dir;
                    }
                }
                return defaultmove(state);

                
            }
            if(mode == Mode.Flee){
                foreach(Direction dir in Enum.GetValues(typeof(Direction))){
                    if(dir.isMoveable() && state.getCell(dir).item == Item.BANANA){
                        return dir;
                    }
                    if(state.getCell(dir).item == Item.PLAYER){
                        if(dir.isMoveable() && state.getCell(dir.opposite()).isFree()){
                            return dir.opposite();
                        }
                        if(dir == Direction.UP_RIGHT || dir == Direction.UP_LEFT && state.getCell(Direction.DOWN).isFree()){
                            return Direction.DOWN;
                        }
                        if(dir == Direction.DOWN_RIGHT || dir == Direction.DOWN_LEFT && state.getCell(Direction.UP).isFree()){
                            return Direction.UP;
                        }
                    }
                }
                return defaultmove(state);

            }
            if(mode == Mode.Hunt){
                return LastDirection;
            }
            else{
                throw new System.Exception("forbidden Mode");
            }
        }
            
        private Direction defaultmove(GameState state){
                List<Direction> freeDirs = computeFreeDirections(state);
                Direction NewDirection;
                Random r = new Random();

                if(LastDirection.isMoveable() && state.getCell(LastDirection).isFree()){ //moves same dir if possible

                    if(r.Next(0, 100) <= 25){ //25% chance of randomly changing direction
                        NewDirection = freeDirs[r.Next(freeDirs.Count)];
                    }
                    else{
                        NewDirection=LastDirection;
                    }
                }
                else{
                    NewDirection = freeDirs[r.Next(freeDirs.Count)];

                    if(NewDirection == LastDirection){  //so the chance of going back is smaller, but not 0
                        NewDirection=freeDirs[r.Next(freeDirs.Count)];
                    }
                }
                LastDirection=NewDirection;
                return NewDirection;
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