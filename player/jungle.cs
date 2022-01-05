using System;

namespace junglestate {
    enum Action {
        STAY = 0,
        MOVE = 1,
        THROW = 2
    }

    enum Direction {
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

    enum Item {
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
                case NONE: return (2,2);
                case UP: return (1,2);
                case UP_RIGHT: return (1,3);
                case RIGHT: return (2,3);
                case DOWN_RIGHT: return (3,3);
                case DOWN: return (3,2);
                case DOWN_LEFT: return (3,1);
                case LEFT: return (2,1);
                case UP_LEFT: return (1,1);
            }
        }

        public static Cell GetCell(this Direction dir, GameState state) {
            var coords = dir.Coordinates();
            return state.cells[coords.Item1][coords.Item2];
        }
    }

    class Move {
        public readonly Action action = Action.STAY;
        public readonly Direction direction = Direction.NONE;
        public readonly string message = "";
        public readonly int nextRound = -1;
    }

    class PlayerInfo {
        public readonly string name;
        public readonly int lives;
        public readonly int coconuts;
        public readonly int points;
    }

    class Cell {
        public readonly Item item;
        public readonly PlayerInfo playerInfo;
        public bool isMoveable() {
            HashSet<Item> moveables = new HashSet<Item>(Item.EMPTY, Item.BANANA, Item.COCONUT, Item.PINEAPPLE);
            return moveables.Contains(item);
        }
    }

    class GameState {
        // 5x5 array of visible cells
        public readonly Cell[][] cells;
        public readonly int round;
        public readonly int lives;
        public readonly int coconuts;
        public readonly int points;
    }

    class BaseMonkey {
        private string name;
        
        BaseMonkey(string name) {
            this.name = name;
        }

        public Move nextMove(GameState state) {
            return new Move{action = Action.STAY, direction = Direction.NONE};
        }
    }
}