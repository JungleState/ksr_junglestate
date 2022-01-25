namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey 
{
    private Direction lastDir = Direction.NONE;
    private bool move_x_priority = true;
    private bool already_changed_priority = false;
    public override Move nextMove(GameState state) 
    {
        if (state.playerInfo.coconuts > 0)
        {
            return new Move(Action.THROW, Direction.UP, state.round, "I bin khul...");
        }
        List<int> coords = getCoordOfNextItem(state);
        Direction dir = Direction.UP;
        int x = coords[1];
        int y = coords[0];
        if (x == -1 && y == -1)
        {
            return makeRandomMove(state);
        }
        dir = getDirectionToMoveFromItemCoords(x, y);
        List<Direction> freeDirs = computeFreeDirections(state);
        if (freeDirs.Contains(dir))
        {
            already_changed_priority = false;
            return new Move(Action.MOVE, dir, state.round, "I han äs Item gfundä");
        }
        Random random = new System.Random();
        Random r = new Random();
        List<Direction> I_hass_C;
        Direction random_dir;
        if (move_x_priority)
        {
            I_hass_C = new List<Direction>{Direction.RIGHT, Direction.LEFT};
            random_dir = I_hass_C[r.Next(I_hass_C.Count)];
            if (freeDirs.Contains(random_dir))
            {
                changeMovePriority();
                return new Move(Action.MOVE, random_dir, state.round, "I ändärä min wäg");
            }
            changeMovePriority();
            return new Move(Action.MOVE, random_dir.opposite(), state.round, "I ändärä min wäg");
        }
        I_hass_C = new List<Direction>{Direction.UP, Direction.DOWN};
        random_dir = I_hass_C[r.Next(I_hass_C.Count)];
        if (freeDirs.Contains(random_dir))
        {
            changeMovePriority();
            return new Move(Action.MOVE, random_dir, state.round, "I ändärä min wäg");
        }
        changeMovePriority();
        return new Move(Action.MOVE, random_dir.opposite(), state.round, "I ändärä min wäg");
        
    }

    private void changeMovePriority()
    {
        if (!already_changed_priority)
        {
            already_changed_priority = true;
            move_x_priority = !move_x_priority;
        }
    }
    private Direction getDirectionToMoveFromItemCoords(int x, int y)
    {
        if (move_x_priority)
        {
            if (x < 2)
            {
                return Direction.LEFT;
            }
            else if (x > 2)
            {
                return Direction.RIGHT;
            }
            else if(y < 2 )
            {
                return Direction.UP;
            }
            return Direction.DOWN;
        }
        if (y < 2)
        {
            return Direction.UP;
        }
        else if (y > 2)
        {
            return Direction.DOWN;
        }
        else if(x < 2 )
        {
            return Direction.LEFT;
        }
        return Direction.RIGHT;
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
        while (direction == lastDir.opposite())
        {
            direction = selectRandomDirection(state);
        }
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