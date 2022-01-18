namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>

public class Monkey : BaseMonkey
{
    private Direction lastDir = Direction.NONE;
    public override Move nextMove(GameState state)
    {
        Direction direction = selectRandomDirection(state);
        lastDir = direction;
        //return new Move(Action.MOVE, direction, state.round, "didn't see this coming, eh?");

        List<String> items = getItemList(state);

        foreach (string name in items)
        {
            Console.WriteLine(name);
        }

        switch (state.playerInfo.lives)
        {
            case 3:
                return new Move(Action.MOVE, direction, state.round, "attack");
            case 2:
                return new Move(Action.MOVE, direction, state.round, "run");
            case 1:
                return new Move(Action.MOVE, direction, state.round, "run run run");
            default:
                return new Move(Action.MOVE, direction, state.round, "error");
        }

    }


    private Direction selectRandomDirection(GameState state)
    {
        List<Direction> freeDirs = computeFreeDirections(state);
        Random random = new System.Random();
        Random r = new Random();
        return freeDirs[r.Next(freeDirs.Count)];
    }

    private List<string> getItemList(GameState state)
    {
        List<Item> ITEM_LIST = new List<Item>{
            {Item.BANANA},
            {Item.COCONUT},
            {Item.PINEAPPLE},
            {Item.PLAYER}
        };

        var items = new List<String> { };
        for (int i = 0; i < state.cells.Length; i++)
        {
            for (int j = 0; j < state.cells[i].Length; j++)
            {
                if ((ITEM_LIST.Contains(state.cells[i][j].item)) && (state.cells[i][j] != state.getCell(Direction.NONE)))
                {
                    items.Add(i + ":" + j + ":" + state.cells[i][j].item);
                }
            }
        }
        return items;
    }

    private List<Direction> computeFreeDirections(GameState state)
    {
        List<Direction> result = new List<Direction>();
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            if (dir.isMoveable() && state.getCell(dir).isFree())
            {
                result.Add(dir);
            }
        }
        return result;
    }
}
