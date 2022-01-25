namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {
    private Direction lastDir = Direction.NONE;
    private int MAX_LIVES = 3;
    private int MAX_AMMO = 3;

    public override Move nextMove(GameState state) {
        // Attack player if possible
        if (state.playerInfo.coconuts > 0 && state.playerInfo.lives > 2) {
            Direction target = directionOfEnemyInRangeWithLowestHealth(state);
            if (target != Direction.NONE) {
                return new Move(Action.THROW, target, state.round, "BAZ!");
            }
        }

        // Collect items
        Direction directionOfItem = directionOfClosestItemOfHighestValue(state);
        if (directionOfItem != Direction.NONE) {
            return new Move(Action.MOVE, directionOfItem, state.round, "Yummy");
        }

        // Otherwise: attempt to move in the same direction as last round (80%)
        Random random = new System.Random();
        Random r = new Random();
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree() && 4 > r.Next(5)) {
            return new Move(Action.MOVE, lastDir, state.round, "...");
        }

        // Otherwise: random move (20%)
        Direction direction = selectRandomDirection(state);
        if (direction == lastDir.opposite()) {
            direction = selectRandomDirection(state);
        }
        lastDir = direction;
        return new Move(Action.MOVE, direction, state.round, "Kan Plan");
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
        // item can be reached in 1 turn
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
            if (dir.isMoveable() && state.getCell(dir).item == Item.PINEAPPLE) {
                return dir;
            }
            if (dir.isMoveable() && state.getCell(dir).item == Item.BANANA) {
                return dir;
            }
            if (dir.isMoveable() && state.getCell(dir).item == Item.COCONUT && state.playerInfo.coconuts < MAX_AMMO) {
                return dir;
            }
        }
        // item can be reached in multiple turns
        (int, int) coordinatesOfBestItem = (404, 404);
        for (int i = 0; i < state.cells.Length; i++) {
            for (int j = 0; j < state.cells[i].Length; j++) {
                if (state.cells[i][j].isFree() && state.cells[i][j].item != Item.EMPTY) {
                    coordinatesOfBestItem = compareTwoItems(state, coordinatesOfBestItem, (i, j));
                }
            }
        }
        // test if any found
        if (coordinatesOfBestItem == (404, 404)) {
            return Direction.NONE;
        }
        // move to item
        Random random = new System.Random();
        Random r = new Random();
        if (r.Next(2) == 0) {
            if (coordinatesOfBestItem.Item1 < 2 && state.cells[1][2].isFree()) {
                return Direction.UP;
            }
            if (coordinatesOfBestItem.Item1 > 2 && state.cells[3][2].isFree()) {
                return Direction.DOWN;
            }
            if (coordinatesOfBestItem.Item2 < 2 && state.cells[2][1].isFree()) {
                return Direction.LEFT;
            }
            if (coordinatesOfBestItem.Item2 > 2 && state.cells[2][3].isFree()) {
                return Direction.RIGHT;
            }
        }
        else {
            if (coordinatesOfBestItem.Item2 < 2 && state.cells[2][1].isFree()) {
                return Direction.LEFT;
            }
            if (coordinatesOfBestItem.Item2 > 2 && state.cells[2][3].isFree()) {
                return Direction.RIGHT;
            }
            if (coordinatesOfBestItem.Item1 < 2 && state.cells[1][2].isFree()) {
                return Direction.UP;
            }
            if (coordinatesOfBestItem.Item1 > 2 && state.cells[3][2].isFree()) {
                return Direction.DOWN;
            }
        }
        
        return Direction.NONE;
    }

    private (int, int) compareTwoItems(GameState state, (int, int) coordinates0, (int, int) coordinates1) {
        // returns the item that is needed most
        int item0Value = 0;
        int item1Value = 0;
        if (coordinates0 != (404, 404)) {
            switch(state.cells[coordinates0.Item1][coordinates0.Item2].item) {
                case Item.PINEAPPLE:
                    item0Value = 3;
                    break;
                case Item.BANANA:
                    if (state.playerInfo.lives < MAX_LIVES || true) {
                        item0Value = 2;
                    }
                    break;
                case Item.COCONUT:
                    if (state.playerInfo.coconuts < MAX_AMMO) {
                        item0Value = 1;
                    }
                    break;
            }
        }        
        switch(state.cells[coordinates1.Item1][coordinates1.Item2].item) {
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
            return coordinates1;
        }
        else {
            return coordinates0;
        }
    }
}

