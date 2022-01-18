namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {
    private Direction lastDir = Direction.NONE;
    public override Move nextMove(GameState state) {
        // Attack player if possible
        if (state.playerInfo.coconuts > 0) {
            Direction target = DirectionOfEnemyInRangeWithLowestHealth(state);
            if (target != Direction.NONE) {
                return new Move(Action.THROW, target, state.round, "RATATATATA!");
            }
        }

        // Collect items
        if (DirectionOfClosestItemOfHighestValue(state) != Direction.NONE) {
            return new Move(Action.MOVE, DirectionOfEnemyInRangeWithLowestHealth(state), state.round, "Items. Items! ITEMS!!");
        }

        // Otherwise: attempt to move in the same direction as last round
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree()) {
            return new Move(Action.MOVE, lastDir, state.round, "Walking is fun.");
        }

        // Otherwise: random move
        Direction direction = selectRandomDirection(state);
        lastDir = direction;
        return new Move(Action.MOVE, direction, state.round, "Where should I go?");
    }

    private Direction selectRandomDirection(GameState state) {
        // select a random direction
        List<Direction> freeDirs = computeFreeDirections(state);
        Random random = new System.Random();
        Random r = new Random();
        return freeDirs[r.Next(freeDirs.Count)];
    }

    private List<Direction> computeFreeDirections(GameState state) {
        // return list with empty
        List<Direction> result = new List<Direction>();
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (dir.isMoveable() && state.getCell(dir).isFree()) {
                result.Add(dir);
            }
        }
        return result;
    }

    private Direction DirectionOfEnemyInRangeWithLowestHealth(GameState state) {
        // returns direction of enemy in range with lowest health
        Direction target = Direction.NONE;
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (state.getCell(dir).item == Item.PLAYER) {
                if (target == Direction.NONE /*|| state.getCell(target).playerInfo!.lives > state.getCell(dir).playerInfo!.lives*/) {
                    target = dir;
                }
            }
        }
        return target;
    }

    private Direction DirectionOfClosestItemOfHighestValue(GameState state) {
        // returns direction of collectable item that is used most
        Direction target = Direction.NONE;
        // look at closest items
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (dir.isMoveable() && state.getCell(dir).item == Item.PINEAPPLE) {
                return dir;
            }
            else if (dir.isMoveable() && state.getCell(dir).item == Item.BANANA && state.playerInfo.lives < 3) {
                return dir;
            }
        }
        return target;
    }

    private Item compareTwoItems(Item item0, Item item1) {
        return item0;
    }
}
