namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey 
{
    private Direction lastDir = Direction.NONE;
    private List<Direction> lastDirsList = new List<Direction>(); 
    public override Move nextMove(GameState state) 
    {
        if (state.playerInfo.coconuts > 0)
        {
            return new Move(Action.THROW, Direction.UP, state.round, "I bin khul...");
        }
        List<int> coords = getCoordOfNextItem(state);
        Action action = Action.MOVE;
        Direction dir = Direction.UP;
        int x = coords[1];
        int y = coords[0];
        if (x == -1 && y == -1)
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
        else if(y < 2 )
        {
            dir = Direction.UP;
        }
        else if (y > 2)
        {
            dir = Direction.DOWN;
        }
        Console.WriteLine(lastDirsList);
        List<Direction> freeDirs = computeFreeDirections(state);
        if (lastDirsList.Count == 5)
        {
            if (lastDirsList[0] == lastDirsList[2] && lastDirsList[2] == lastDirsList[4] && lastDirsList[1] == lastDirsList[3])
            {
                if (lastDirsList[0] == Direction.UP || lastDirsList[0] == Direction.DOWN) 
                {
                    dir = Direction.RIGHT;
                }
                else
                {
                    dir = Direction.UP;
                }
            }
        }   
        if (freeDirs.Contains(dir))
        {
            return makeMove(action, dir, state, "I han äs Item gfundä...");
        }
        else if (x == 2)
        {
            if (Direction.RIGHT.isMoveable())
            {
                return makeMove(Action.MOVE, Direction.RIGHT, state, "I han äs Item gfundä...");
            }
            return makeMove(Action.MOVE, Direction.LEFT, state, "I han äs Item gfundä...");
                
        }
        else if (y == 2)
        {
            if (Direction.UP.isMoveable())
            {
                return makeMove(Action.MOVE, Direction.UP, state, "I han äs Item gfundä...");
            }
            return makeMove(Action.MOVE, Direction.DOWN, state, "I han äs Item gfundä...");
        }
        return makeRandomMove(state);
        
    }
    private List<Direction> updateLastDirsList(GameState state, List<Direction> ldl, Direction newDir)
    {
        ldl.Add(newDir);
        if (ldl.Count == 6)
        {
            ldl.RemoveAt(0);
        }
        return ldl;

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

    private Move makeMove(Action action, Direction dir, GameState state, string msg)
    {
        lastDirsList = updateLastDirsList(state, lastDirsList, dir);
        return new Move(action, dir, state.round, msg);
    }

    private Move makeRandomMove(GameState state)
    {
        if (lastDir.isMoveable() && state.getCell(lastDir).isFree()) 
        {
            return new Move(Action.MOVE, lastDir, state.round, "I suäch ässä...");
        }
        Direction direction = selectRandomDirection(state);
        while (direction == lastDir.opposite())
        {
            direction = selectRandomDirection(state);
        }
        lastDir = direction;
        lastDirsList = updateLastDirsList(state, lastDirsList, direction);
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