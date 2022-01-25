namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {

    private Direction defaultDir = Direction.RIGHT;
    private string monkeyMode = "walk";

    public override Move nextMove(GameState state) {
        List<Direction> freeDirs = computeFreeDirections(state);
        Direction nextDir;
        Random r = new Random();
        
        nextDir = getItem("", state, freeDirs);

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
        if (item == Item.PLAYER) { // Attack: Ignore if no lives / no coconuts
            if (state.playerInfo.lives == 1 || state.playerInfo.coconuts == 0) {
                return false;
            }
        }
        else if (item == Item.BANANA) { // Banana: Ignore if 3 lives
            if (state.playerInfo.lives == 3) {
                return false;
            } 
        }
        else if (item == Item.COCONUT) { // Coconut: Ignore if 3 coconuts
            if (state.playerInfo.coconuts == 3) {
                return false;
            }
        }
        else if (item == Item.FOREST || item == Item.EMPTY) { // Empty Cell: ignore
            return false;
        }

        return true;
    }

    private Direction getItem(string type, GameState state, List<Direction> freeDirs) {
        List<(int, int)> coords = new List<(int, int)>();
        Direction newDir = Direction.NONE;
        for (int i = 0; i < state.cells.Length; i++) {
            for (int j = 0; j < state.cells[i].Length; j++) {
                if (decideMove(state, state.cells[i][j].item)) {
                    coords.Add((i, j));
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