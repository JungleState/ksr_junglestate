namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey {

    private Direction defaultDir = Direction.RIGHT;
    private string monkeyMode = "walk";
    private bool chasingItem = false;
    private bool attack = false;

    public override Move nextMove(GameState state) {
        List<Direction> freeDirs = computeFreeDirections(state);
        Direction nextDir;
        Random r = new Random();
        
        nextDir = getItem("", state, freeDirs, r);

        if (nextDir == Direction.NONE) {
            if (this.monkeyMode == "walk") {
                if (freeDirs.Contains(this.defaultDir)) {
                    nextDir = this.defaultDir;
                    if (r.Next(5) == 0) {
                        // 25% chance to change direction
                        nextDir = newRandomDirection(freeDirs, r);
                    }
                }
                else {
                    nextDir = newRandomDirection(freeDirs, r);
                }

                return new Move(Action.MOVE, nextDir, state.round, "Food?");
            }
        }
        else {
            System.Console.WriteLine("Run for food");
            if (this.attack) {
                return new Move(Action.THROW, nextDir, state.round, "FOOOD");
            }
            else {
                return new Move(Action.MOVE, nextDir, state.round, "FOOOD");
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

    private Direction getItem(string type, GameState state, List<Direction> freeDirs, Random r) {
        List<Item> food = new List<Item> {Item.BANANA, Item.COCONUT, Item.PINEAPPLE};
        List<(int, int)> coords = new List<(int, int)>();
        Direction newDir = Direction.NONE;
        for (int i = 0; i < state.cells.Length; i++) {
            for (int j = 0; j < state.cells[i].Length; j++) {
                if (type == "" && food.Contains(state.cells[i][j].item)) {
                    if (state.cells[i][j].item == Item.COCONUT && state.playerInfo.coconuts < 3) {
                        coords.Add((i, j));
                    }
                }
                else if (type == "" && state.cells[i][j].item == Item.PLAYER) {
                    coords.Add((i, j));
                }
            }
        }

        coords.ForEach(dir => {
            if (dir.Item2 == 2) {
                if (dir.Item1 < 2) {
                    if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
                        attack = true;
                    }
                    else {
                        attack = false;
                    }
                    System.Console.WriteLine("Item on top");
                    newDir = Direction.UP;
                    this.chasingItem = true;
                    return;
                }
                else if (dir.Item1 > 2) {
                    if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
                        attack = true;
                    }
                    else {
                        attack = false;
                    }
                    System.Console.WriteLine("Item on bottom");
                    newDir = Direction.DOWN;
                    this.chasingItem = true;
                    return;
                }
            }
            else if (dir.Item2 < 2 || dir.Item2 > 2) {
                if (dir.Item1 == 2 && dir.Item2 < 2) {
                    if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
                        attack = true;
                    }
                    else {
                        attack = false;
                    }
                    System.Console.WriteLine("Item left");
                    newDir = Direction.LEFT;
                    this.chasingItem = true;
                    return;
                }
                else if (dir.Item1 == 2 && dir.Item2 > 2) {
                    if (state.cells[dir.Item1][dir.Item2].item == Item.PLAYER) {
                        attack = true;
                    }
                    else {
                        attack = false;
                    }
                    System.Console.WriteLine("Item right");
                    newDir = Direction.RIGHT;
                    this.chasingItem = true;
                    return;
                }
            }

            if (dir.Item1 == 2 && dir.Item2 == 2) {
                this.chasingItem = false;
                return;
            }
        }); 

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