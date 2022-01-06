using System.Collections.Generic;
using System.Collections.Immutable;

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

    public static class ItemInfo {
        private static readonly ISet<Item> MOVEABLE_ITEMS = ImmutableHashSet.Create(Item.EMPTY, Item.BANANA, Item.COCONUT, Item.PINEAPPLE);

        public static bool isMoveable(this Item item) {
            return MOVEABLE_ITEMS.Contains(item);
        }

        public static Item fromCode(string code) {
            switch (code) {
                case "FF":
                    return Item.FOREST;
                case "  ":
                    return Item.EMPTY;
                case "PP":
                    return Item.PINEAPPLE;
                case "BB":
                    return Item.BANANA;
                case "CC":
                    return Item.COCONUT;
                default:
                    int number;
                    if (!int.TryParse(code, out number)) {
                        throw new ArgumentException($"Unexpectec cell kind: {code}");
                    }
                    return Item.PLAYER;
            }
        }
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

    public sealed class Move {
        public Action action = Action.STAY;
        public Direction direction = Direction.NONE;
        public string message = "";
        public int nextRound = -1;
    }

    public sealed class PlayerInfo {
        public PlayerInfo(string name, int lives, int coconuts, int points) {
            this.name = name;
            this.lives = lives;
            this.coconuts = coconuts;
            this.points = points;
        }
        public readonly string name;
        public readonly int lives;
        public readonly int coconuts;
        public readonly int points;
    }

    public sealed class Cell {
        private static readonly Cell EMPTY = new Cell(Item.EMPTY, null);
        private static readonly Cell PINEAPPLE = new Cell(Item.PINEAPPLE, null);
        private static readonly Cell BANANA = new Cell(Item.BANANA, null);
        private static readonly Cell COCONUT = new Cell(Item.COCONUT, null);
        public static Cell ItemCell(Item item) {
            switch(item) {
                case Item.EMPTY:
                    return EMPTY;
                case Item.PINEAPPLE:
                    return PINEAPPLE;
                case Item.BANANA:
                    return BANANA;
                case Item.COCONUT:
                    return COCONUT;
            }
            throw new ArgumentException($"Unknown item type: {item}");
        }

        public static Cell PlayerCell(PlayerInfo playerInfo) {
            return new Cell(Item.PLAYER, playerInfo);
        }
        private Cell(Item item, PlayerInfo? playerInfo) {
            this.item = item;
            this.playerInfo = playerInfo;
        }
        public readonly Item item;
        public readonly PlayerInfo? playerInfo;
        public bool isMoveable() {
            return item.isMoveable();
        }
    }

    public sealed class GameState {
        public GameState(Cell[][] cells, int round, int lives, int coconuts, int points) {
            this.cells = cells;
            this.round = round;
            this.lives = lives;
            this.coconuts = coconuts;
            this.points = points;
        }
        // 5x5 array of visible cells
        public readonly Cell[][] cells;
        public readonly int round;
        public readonly int lives;
        public readonly int coconuts;
        public readonly int points;
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