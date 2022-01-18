namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {

    private Direction defaultDir = Direction.RIGHT;
    private string monkeyMode = "walk";

    public override Move nextMove(GameState state) {
        List<Direction> freeDirs = computeFreeDirections(state);
        Direction nextDir;
        Random r = new Random();
        
        getFood(state, freeDirs);

        if (this.monkeyMode == "walk") {
            if (freeDirs.Contains(this.defaultDir)) {
                nextDir = this.defaultDir;
                if (r.Next(4) == 0) {
                    // 25% chance to change direction
                    nextDir = newRandomDirection(freeDirs, r);
                }
            }
            else {
                nextDir = newRandomDirection(freeDirs, r);
            }

            return new Move(Action.MOVE, nextDir, state.round);
        }

        return new Move(Action.MOVE, Direction.NONE, state.round);
    }

    private Direction newRandomDirection(List<Direction> freeDirs, Random r) {
        Direction nextDir;
        while (true) {
            nextDir = freeDirs[r.Next(freeDirs.Count)];
            if (nextDir.opposite() != defaultDir || freeDirs.Count == 1) {
                this.defaultDir = nextDir;
                break;
            }
        }

        return nextDir;
    }

    private Direction getFood(GameState state, List<Direction> freeDirs) {
        for (int i = 0; i < state.cells.Count; i++) {
            
        }
        return Direction.NONE;
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
            if (dir.isMoveable() && state.getCell(dir).isFree()) {
                result.Add(dir);
            }
        }
        return result;
    }
}