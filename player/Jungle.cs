using System.Collections.Generic;
using System.Collections.Immutable;

namespace junglestate {
    /// <summary>The possible actions of a monkey in a round.</summary>
    public enum Action {
        STAY = 0,
        MOVE = 1,
        THROW = 2
    }

    /// <summary>The possible directions of an <see cref="Action"/>.</summary>
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

    /// <summary>Utility methods pertaining to <see cref="Item"/>.</summary>
    public static class ItemInfo {
        private static readonly ISet<Item> MOVEABLE_ITEMS = ImmutableHashSet.Create(Item.EMPTY, Item.BANANA, Item.COCONUT, Item.PINEAPPLE);

        /// <summary>Returns true exactly if a monkey can move to a <see cref="Cell"/> containing this item.</summary>
        public static bool isMoveable(this Item item) {
            return MOVEABLE_ITEMS.Contains(item);
        }

        /// <summary>Returns an <see cref="Item"/> matching the given two-letter code.</summary>
        /// <exception cref="ArgumentExceptoin">if the given code is unknown.</exception>
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

    /// <summary>The possible contents of a <see cref="Cell"/>.</summary>
    public enum Item {
        EMPTY,
        FOREST,
        BANANA,
        COCONUT,
        PINEAPPLE,
        PLAYER
    }

    /// <summary>Utility methods pertaining to <see cref="Direction"/>.</summary>
    public static class DirectionInfo {
        /// <summary>Returns the x and y coordinates into a 5x5 cell array corresponding to <paramref name="dir"/>.</summary>
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

        public static bool isMoveable(this Direction dir) {
            switch (dir) {
                case Direction.NONE:
                case Direction.UP:
                case Direction.RIGHT:
                case Direction.DOWN:
                case Direction.LEFT:
                    return true;
            }
            return false;
        }
    }

    /// <summary>The description of a monkey's move in the next round.</summary>
    public sealed class Move {
        public Move(Action action, Direction direction, int nextRound = -1, string message = "") {
            this.action = action;
            this.direction = direction;
            this.nextRound = nextRound;
            this.message = message;
        }
        ///<summary>The <see cref="Action"/> to take.</summary>
        public readonly Action action = Action.STAY;
        ///<summary>The <see cref="Direction"/> into which to act.</summary>
        public readonly Direction direction = Direction.NONE;
        ///<summary>The optional message to display in this round.</summary>
        public readonly string message = "";
        ///<summary>The round identifier - allows the game manager to detect out-of-sync moves.</summary>
        public readonly int nextRound = -1;
    }

    /// <summary>The description of another player in the monkey's view.</summary>
    public sealed class PlayerInfo {
        public PlayerInfo(string name, int lives, int coconuts, int points) {
            this.name = name;
            this.lives = lives;
            this.coconuts = coconuts;
            this.points = points;
        }
        /// <summary>The name of the player.</summary>
        public readonly string name;
        /// <summary>The number of lives remaining for the player.</summary>
        public readonly int lives;
        /// <summary>The number of coconuts remaining for the player.</summary>
        public readonly int coconuts;
        /// <summary>The number of points scored by player.</summary>
        public readonly int points;
    }

    /// <summary>The description of a Cell in the monkey's environment.</summary>
    public sealed class Cell {
        private static readonly Cell EMPTY = new Cell(Item.EMPTY, null);
        private static readonly Cell PINEAPPLE = new Cell(Item.PINEAPPLE, null);
        private static readonly Cell FOREST = new Cell(Item.FOREST, null);
        private static readonly Cell BANANA = new Cell(Item.BANANA, null);
        private static readonly Cell COCONUT = new Cell(Item.COCONUT, null);
        /// <summary>Creates a cell containing a regular item (or empty), not a player.</summary>
        public static Cell ItemCell(Item item) {
            switch(item) {
                case Item.EMPTY:
                    return EMPTY;
                case Item.PINEAPPLE:
                    return PINEAPPLE;
                case Item.FOREST:
                    return FOREST;
                case Item.BANANA:
                    return BANANA;
                case Item.COCONUT:
                    return COCONUT;
            }
            throw new ArgumentException($"Unknown item type: {item}");
        }

        /// <summary>Creates a cell containing a player.</summary>
        public static Cell PlayerCell(PlayerInfo playerInfo) {
            return new Cell(Item.PLAYER, playerInfo);
        }
        private Cell(Item item, PlayerInfo? playerInfo) {
            this.item = item;
            this.playerInfo = playerInfo;
        }
        ///<summary>The item kind contained by this cell.</summary>
        public readonly Item item;
        ///<summary>The optional player info, <c>null</c> unless <see cref="item"/> is <see cref="Item.PLAYER"/>.</summary>
        public readonly PlayerInfo? playerInfo;
        ///<summary>Returns true exactly if this cell can probably be moved to in this round.</summary>
        public bool isFree() {
            return item.isMoveable();
        }
    }

    ///<summary>The description of the game state visible to the monkey in a given round.</summary>
    public sealed class GameState {
        public GameState(Cell[][] cells, int round, PlayerInfo playerInfo) {
            this.cells = cells;
            this.round = round;
            this.playerInfo = playerInfo;
        }
        ///<summary>The 5x5 array of cells visible around the monkey's position.</summary>
        public readonly Cell[][] cells;
        ///<summary>The game round identifier.</summary>
        public readonly int round;
        ///<summary>The monkey's own player information.</summary>
        public readonly PlayerInfo playerInfo;

        /// <summary>Returns the <see cref="Cell"/> at the given direction from the center of this state's 5x5 view.</summary>
        public Cell getCell(Direction dir) {
            var coords = dir.Coordinates();
            return cells[coords.Item1][coords.Item2];
        }
    }

    /// <summary>The most trivial monkey implementation that will always <see cref="Action.STAY"/> where it is.</summary>
    /// <remarks>Extend this class to make it smarter.</remarks>
    /// <seealso cref="Monkey"/>
    public class BaseMonkey {
        ///<summary>The monkey's name.</summary>
        public readonly string name;
        
        public BaseMonkey(string name) {
            this.name = name;
        }

        /// <summary>Defines the move that the monkey takes in a given round.</summary>
        /// <remarks>Override or extend this method to refine the monkey's behavior.</remarks>
        public virtual Move nextMove(GameState state) {
            return new Move(Action.STAY, Direction.NONE, state.round);
        }
    }
}