namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {
    private Direction lastDir = Direction.NONE;
    private int MAX_LIVES = 3;
    private int MAX_AMMO = 3;

    public override Move nextMove(GameState state) {
        // Attack player if possible
        if (state.playerInfo.coconuts > 0) {
            Direction target = directionOfEnemyInRangeWithLowestHealth(state);
            if (target != Direction.NONE) {
                return new Move(Action.THROW, target, state.round, "RATATATATA!");
            }
        }

        // Collect items
        Direction directionOfItem = directionOfClosestItemOfHighestValue(state);
        if (directionOfItem != Direction.NONE) {
            return new Move(Action.MOVE, directionOfItem, state.round, "Items. Items! ITEMS!!");
        }

        // Otherwise: attempt to move in the same direction as last round (80%)
        Random random =new System.Random();
        Random r = new Random();
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree() && 4 > r.Next(5)) {
            return new Move(Action.MOVE, lastDir, state.round, "Walking is fun.");
        }

        // Otherwise: random move (20%)
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

    private Direction directionOfEnemyInRangeWithLowestHealth(GameState state) {
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

    private Direction directionOfClosestItemOfHighestValue(GameState state) {
        // returns direction of collectable item that is used most
        Direction target = Direction.NONE;
        // item can be reached in 1 turn
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (dir.isMoveable() && state.getCell(dir).item == Item.PINEAPPLE) {
                return dir;
            }
            if (dir.isMoveable() && state.getCell(dir).item == Item.BANANA && state.playerInfo.lives < MAX_LIVES) {
                return dir;
            }
            if (dir.isMoveable() && state.getCell(dir).item == Item.COCONUT && state.playerInfo.coconuts < MAX_AMMO) {
                return dir;
            }
        }
        // item can be reached in 2 turns
        List<Direction> direction2List = new List<Direction>();




        return target;
    }

    private Item compareTwoItems(GameState state, Item item0, Item item1) {
        // returns the item that is needed most
        int item0Value = 0;
        int item1Value = 0;
        switch(item0) {
            case Item.PINEAPPLE:
                item0Value = 3;
                break;
            case Item.BANANA:
                if (state.playerInfo.lives < MAX_LIVES) {
                    item0Value = 2;
                }
                break;
            case Item.COCONUT:
                if (state.playerInfo.coconuts < MAX_AMMO) {
                    item0Value = 1;
                }
                break;
        }
        switch(item1) {
            case Item.PINEAPPLE:
                item1Value = 3;
                break;
            case Item.BANANA:
                if (state.playerInfo.lives < MAX_LIVES) {
                    item1Value = 2;
                }
                break;
            case Item.COCONUT:
                if (state.playerInfo.coconuts < MAX_AMMO) {
                    item1Value = 1;
                }
                break;
        }
        if (item1Value > item0Value) {
            return item1;
        }
        else {
            return item0;
        }
    }
}
