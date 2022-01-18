namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey 
{
    private Direction lastDir = Direction.NONE;
    public override Move nextMove(GameState state) 
    {
        if (state.playerInfo.coconuts > 0)
        {
            return new Move(Action.THROW, Direction.UP, state.round, "I bin khul...");
        }
        List<int> coords = getCoordOfNextItem(state);
        Action action = Action.MOVE;
        Direction dir = Direction.UP;
        int x = coords[0];
        int y = coords[1];
        if (x == -1)
        {
           return makeRandomMove(state);
        }
        if (x < 2)
        {
            dir = Direction.LEFT;
        }
        else if (x > 2)
        {
            dir = Direction.RIGHT;
        }
        else if (y < 2)
        {
            dir = Direction.UP;
        }
        else if (x > 2)
        {
            dir = Direction.DOWN;
        }
        List<Direction> freeDirs = computeFreeDirections(state);
        if (freeDirs.Contains(dir))
        {
            return new Move(action, dir, state.round, "I han äs Item gfundä...");
        }
        return makeRandomMove(state);
        
    }

    private List<int> getCoordOfNextItem(GameState state)
    {  
        for (int row = 0; row < 5; row++)
        {
            for (int column = 0; column < 5; column++)
            {   
                switch (state.cells[row][column].item)
                {
                    case Item.BANANA:
                    case Item.PINEAPPLE:
                        return new List<int> {row, column};
                }
            }
        }
        return new List<int> {-1, -1};
    }

    private Move makeRandomMove(GameState state)
    {
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree()) 
        {
            return new Move(Action.MOVE, lastDir, state.round, "I suäch ässä...");
        }
        Direction direction = selectRandomDirection(state);
        lastDir = direction;
        return new Move(Action.MOVE, direction, state.round, "I suäch ässä...");
    }

    private Direction selectRandomDirection(GameState state) 
    {
        List<Direction> freeDirs = computeFreeDirections(state);
        Random random = new System.Random();
        Random r = new Random();
        return freeDirs[r.Next(freeDirs.Count)];
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