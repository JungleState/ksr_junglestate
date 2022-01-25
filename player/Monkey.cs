namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {

    private Direction defaultDir = Direction.RIGHT;
    private string monkeyMode = "walk";

    public override Move nextMove(GameState state) {
        List<Direction> freeDirs = computeFreeDirections(state);
        Direction nextDir;
        Random r = new Random();
        
        nextDir = getItem("", state, freeDirs, r);

        System.Console.WriteLine(nextDir);

        if (nextDir == Direction.NONE || state.getCell(nextDir).item == Item.FOREST) {
            if (this.monkeyMode == "walk") {
                if (freeDirs.Contains(this.defaultDir)) {
                    if (r.Next(5) == 0) {
                        // 25% chance to change direction
                        nextDir = newRandomDirection(freeDirs, r);
                    }
                    nextDir = this.defaultDir;
                }
                else {
                    nextDir = newRandomDirection(freeDirs, r);
                }

                return new Move(Action.MOVE, nextDir, state.round, "Just walking...");
            }
        }
        else {
            if (this.monkeyMode == "attack") {
                return new Move(Action.THROW, nextDir, state.round, "Attack!!");
            }
            else {
                return new Move(Action.MOVE, nextDir, state.round, "Yummy");
            }
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

    private bool decideMove(GameState state, Item item) {
        // if (item == Item.PLAYER) { // Attack: Ignore if no lives / no coconuts
        //     if (state.playerInfo.lives == 1 || state.playerInfo.coconuts == 0) {
        //         return false;
        //     }
        // }
        // if (item == Item.BANANA) { // Banana: Ignore if 3 lives
        //     if (state.playerInfo.lives == 3) {
        //         return false;
        //     } 
        // }
        // else if (item == Item.COCONUT) { // Coconut: Ignore if 3 coconuts
        //     if (state.playerInfo.coconuts == 3) {
        //         return false;
        //     }
        // }
        if (item == Item.FOREST || item == Item.EMPTY) { // Empty Cell: ignore
            return false;
        }

        return true;
    }

    private Direction getItem(string type, GameState state, List<Direction> freeDirs, Random r) {
        List<(int, int)> coords = new List<(int, int)>();
        Direction newDir = Direction.NONE;
        for (int i = 0; i < state.cells.Length; i++) {
            for (int j = 0; j < state.cells[i].Length; j++) {
                if (decideMove(state, state.cells[i][j].item)) {
                    coords.Add((i, j));
                    System.Console.WriteLine(state.cells[i][j].item);
                }
            }
        }

        for (int i = 0; i < coords.Count; i++) {
            if (state.cells[coords[i].Item1][coords[i].Item2].item == Item.PLAYER) {
                if (!(coords[i].Item1 == 2 && coords[i].Item2 == 2)) {

                    string[] place = new string[2];

                    if (coords[i].Item2 < 2) {
                        System.Console.WriteLine("Item Left");
                        newDir = Direction.LEFT;
                        place[1] = "Left";
                    }
                    else if (coords[i].Item2 > 2) {
                        System.Console.WriteLine("Item Right");
                        newDir = Direction.RIGHT;
                        place[1] = "Right";
                    }
                    if (coords[i].Item1 < 2) {
                        System.Console.WriteLine("Item Above");
                        newDir = Direction.UP;
                        place[0] = "Up";
                    }
                    else if (coords[i].Item1 > 2) {
                        Console.WriteLine("Item Below");
                        newDir = Direction.DOWN;
                        place[0] = "Down";
                    }

                    if (place[0] != "" && place[1] != "") {
                        System.Console.WriteLine("Diagonal");
                    }
                    else {
                        System.Console.WriteLine("Straight");
                    }
                }
            }
            else { // Eatable (relevant) Item
                if (!(coords[i].Item1 == 2 && coords[i].Item2 == 2)) {

                    string[] place = new string[2];

                    if (coords[i].Item2 < 2) {
                        System.Console.WriteLine("Item Left");
                        place[1] = "Left";
                    }
                    else if (coords[i].Item2 > 2) {
                        System.Console.WriteLine("Item Right");
                        place[1] = "Right";
                    }
                    if (coords[i].Item1 < 2) {
                        System.Console.WriteLine("Item Above");
                        place[0] = "Up";
                    }
                    else if (coords[i].Item1 > 2) {
                        System.Console.WriteLine("Item Below");
                        place[0] = "Down";
                    }

                    if (place[0] != "" && place[1] != "") {
                        int decision = r.Next(2);
                        if (decision == 0) {
                            if (place[0] == "Down") {
                                newDir = Direction.DOWN;
                            }
                            else {
                                newDir = Direction.UP;
                            }
                        }
                        else {
                            if (place[1] == "Left") {
                                newDir = Direction.LEFT;
                            }
                            else {
                                newDir = Direction.RIGHT;
                            }
                        }
                    }
                    else {
                        if (place[0] == "Down") {
                            newDir = Direction.DOWN;
                        }
                        else if (place[0] == "Up") {
                            newDir = Direction.UP;
                        }
                        else if (place[1] == "Left") {
                            newDir = Direction.LEFT;
                        }
                        else if (place[1] == "Right") {
                            newDir = Direction.RIGHT;
                        }
                    }
                }
            }
        }

        // coords.ForEach(dir => {
        //     if (dir.Item2 == 2) {
        //         if (dir.Item1 < 2) {
        //             if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
        //                 this.monkeyMode = "attack";
        //             }
        //             else {
        //                 this.monkeyMode = "walk";
        //             }
        //             System.Console.WriteLine("Item on top");
        //             newDir = Direction.UP;
        //             return;
        //         }
        //         else if (dir.Item1 > 2) {
        //             if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
        //                 this.monkeyMode = "attack";
        //             }
        //             else {
        //                 this.monkeyMode = "walk";
        //             }
        //             System.Console.WriteLine("Item on bottom");
        //             newDir = Direction.DOWN;
        //             return;
        //         }
        //     }
        //     else if (dir.Item2 < 2 || dir.Item2 > 2) {
        //         if (dir.Item1 == 2 && dir.Item2 < 2) {
        //             if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
        //                 this.monkeyMode = "attack";
        //             }
        //             else {
        //                 this.monkeyMode = "walk";
        //             }
        //             System.Console.WriteLine("Item left");
        //             newDir = Direction.LEFT;
        //             return;
        //         }
        //         else if (dir.Item1 == 2 && dir.Item2 > 2) {
        //             if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
        //                 this.monkeyMode = "attack";
        //             }
        //             else {
        //                 this.monkeyMode = "walk";
        //             }
        //             System.Console.WriteLine("Item right");
        //             newDir = Direction.RIGHT;
        //             return;
        //         }
        //     }

        //     if (dir.Item1 == 2 && dir.Item2 == 2) {
        //         return;
        //     }
        // }); 

        return newDir;
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