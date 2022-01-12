namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>

public class Monkey : BaseMonkey {
    private Direction lastDir = Direction.NONE;
    public override Move nextMove(GameState state) {
        // Attempt to move in the same direction as last round.
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree()) {
            return new Move(Action.MOVE, lastDir, state.round, "it's so boring...");
        }
        // Otherwise: random move
        Direction direction = selectRandomDirection(state);
        lastDir = direction;
        return new Move(Action.MOVE, direction, state.round, "didn't see this coming, eh?");
        int i = 3;
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
