namespace junglestate {
    public enum Action {
        STAY = 0,
        MOVE = 1,
        THROW = 2
    }

    public enum Direction {
        NONE = -1,
        UP = 0,
        UP_RIGHT = 1,
        RIGHT = 2,
        DOWN_RIGHT = 3,
        DOWN = 4,
        DOWN_LEFT = 5,
        LEFT = 6,
        UP_LEFT = 7
    }

    public enum Item {
        EMPTY,
        FOREST,
        BANANA,
        COCONUT,
        PINEAPPLE,
        PLAYER
    }

    public static class DirectionInfo {
        public static (int, int) Coordinates(this Direction dir) {
            switch (dir) {
                case Direction.NONE: return (2,2);
                case Direction.UP: return (1,2);
                case Direction.UP_RIGHT: return (1,3);
                case Direction.RIGHT: return (2,3);
                case Direction.DOWN_RIGHT: return (3,3);
                case Direction.DOWN: return (3,2);
                case Direction.DOWN_LEFT: return (3,1);
                case Direction.LEFT: return (2,1);
                case Direction.UP_LEFT: return (1,1);
            }
            return (2,2);
        }

        public static Cell GetCell(this Direction dir, GameState state) {
            var coords = dir.Coordinates();
            return state.cells[coords.Item1][coords.Item2];
        }
    }

    public class Move {
        public Action action = Action.STAY;
        public Direction direction = Direction.NONE;
        public string message = "";
        public int nextRound = -1;
    }

    public class PlayerInfo {
        public string name;
        public int lives;
        public int coconuts;
        public int points;
    }

    public class Cell {
        public Item item;
        public PlayerInfo playerInfo;
        public bool isMoveable() {
            HashSet<Item> moveables = new HashSet<Item>(){Item.EMPTY, Item.BANANA, Item.COCONUT, Item.PINEAPPLE};
            return moveables.Contains(item);
        }
    }

    public class GameState {
        // 5x5 array of visible cells
        public Cell[][] cells;
        public int round;
        public int lives;
        public int coconuts;
        public int points;
    }

    public class BaseMonkey {
        public readonly string name;
        
        public BaseMonkey(string name) {
            this.name = name;
        }

        public virtual Move nextMove(GameState state) {
            return new Move{action = Action.STAY, direction = Direction.NONE};
        }
    }
}