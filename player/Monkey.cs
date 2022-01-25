namespace junglestate;

///<summary>A simple monkey that moves randomly in free directions.</summary>
public class Monkey : BaseMonkey 
{
    private Direction lastDir = Direction.NONE;
    private List<(int, int, int)> items = new List<(int, int, int)> {};
    private List<List<int>> paths = new List<List<int>> {};
    private List<Direction> current_path = new List<Direction> {};
    public override Move nextMove(GameState state) 
    {
        
        if (current_path.Count == 0)
        {
            items = getCoordsOfItems(state);
            foreach ((int, int, int) item in items)
            {
                paths.Add(findPath(state, item.Item1, item.Item2, item.Item3)); 
            }
            if (state.playerInfo.lives < 3)
            {
                foreach(List<int> path in paths)
                {
                    if(path[0] == 1)
                    {
                        path.RemoveAt(0);
                        current_path = convertToDirectionPath(path);
                        break;
                    }
                }
            }
            if (state.playerInfo.coconuts == 0 && current_path.Count == 0)
            {
                foreach(List<int> path in paths)
                {
                    if(path[0] == 0)
                    {
                        path.RemoveAt(0);
                        current_path = convertToDirectionPath(path);
                        break;
                    }
                }   
            }
            else//take next best path to item
            {
                foreach(List<int> path in paths)
                {
                    if(path[0] != 0)
                    {
                        path.RemoveAt(0);
                        current_path = convertToDirectionPath(path);
                        break;
                    }
                }  
            }
        }
        if (current_path.Count != 0)
        {
            lastDir = current_path[0];
            current_path.RemoveAt(0);
            return new Move(Action.MOVE, lastDir, state.round, "I bin khul...");
        }
        return makeRandomMove(state);
        
    }


    private List<(int, int, int)> getCoordsOfItems(GameState state)
    {  
        //returns list of Item coords (item_id, x, y)
        //ids: 0=coconut, 1=banana, 2=pinapple
        List<(int, int, int)> items = new List<(int, int, int)> {};
        for (int row = 0; row < 5; row++)
        {
            for (int column = 0; column < 5; column++)
            {
                switch (state.cells[row][column].item)
                {
                    case Item.COCONUT:
                        items.Add((0, row, column));
                        break;
                    case Item.BANANA:
                        items.Add((1, row, column));
                        break;
                    case Item.PINEAPPLE:
                        items.Add((2, row, column));   
                        break;        
                }       
            }
        }
        return items;
    }

    private List<Direction> convertToDirectionPath(List<int> int_path)
    {
        List<Direction> dir_path = new List<Direction> {};
        foreach (int dir in int_path)
        {
            switch (dir)
            {
                case 1:
                    dir_path.Add(Direction.UP);
                    break;
                case 2:
                    dir_path.Add(Direction.RIGHT);
                    break;
                case 3:
                    dir_path.Add(Direction.DOWN);
                    break;
                case 4:
                    dir_path.Add(Direction.LEFT);
                    break;
            }
        }
        return dir_path;
    }
    private List<int> findPath(GameState state, int item, int x, int y)
    {
        int item_x = x;
        int item_y = y;
        List<int> path = new List<int> {item};
        List<(int, int)> path_cells = new List<(int, int)> {(2, 2)};
        int player_x = path_cells[0].Item1;
        int player_y = path_cells[0].Item2;
        List<(int, int)> add_new_possible_cells_to_go = new List<(int, int)> {};
        List<(int, int)> new_possible_cells_to_go = new List<(int, int)> {(x, y)};
        List<(int, int)> possible_cells_to_go = new List<(int, int)> {};
        List<(int, int)> next_move_cells = new List<(int, int)> {};
        bool found_path = false;
        //makes list with all possible path cells
        int j = 0;
        while (true)
        {
            if (found_path)
            {
                break;
            }
            foreach ((int, int) cell_coords in new_possible_cells_to_go)
            {

                x = cell_coords.Item1;
                y = cell_coords.Item2;
                if (x != 4)
                {
                    if (state.cells[x+1][y].isFree())
                    {
                        if (!new_possible_cells_to_go.Contains((x+1, y)) && !possible_cells_to_go.Contains((x+1, y)) && !add_new_possible_cells_to_go.Contains((x+1, y)))
                        {
                            add_new_possible_cells_to_go.Add((x+1, y));
                        }
                    }
                }
                if (x != 0)
                {
                    if (state.cells[x-1][y].isFree())
                    {
                        if (!new_possible_cells_to_go.Contains((x-1, y)) && !possible_cells_to_go.Contains((x-1, y)) && !add_new_possible_cells_to_go.Contains((x-1, y)))
                        {
                            add_new_possible_cells_to_go.Add((x-1, y));
                        }
                    }
                }
                if (y != 4)
                {
                    if (state.cells[x][y+1].isFree())
                    {
                        if (!new_possible_cells_to_go.Contains((x, y+1)) && !possible_cells_to_go.Contains((x, y+1)) && !add_new_possible_cells_to_go.Contains((x, y+1)))
                        {
                            add_new_possible_cells_to_go.Add((x, y+1));
                        }
                    }
                }
                if (y != 0)
                {
                    if (state.cells[x][y-1].isFree())
                    {
                        if (!new_possible_cells_to_go.Contains((x, y-1)) && !possible_cells_to_go.Contains((x, y-1)) && !add_new_possible_cells_to_go.Contains((x, y-1)))
                        {
                            add_new_possible_cells_to_go.Add((x, y-1));
                        }
                    }
                }
            }
            foreach((int, int) coord in new_possible_cells_to_go)
            {
                possible_cells_to_go.Add(coord);
            }
            new_possible_cells_to_go.Clear();
            foreach((int, int) coord in add_new_possible_cells_to_go)
            {
                new_possible_cells_to_go.Add(coord);
            }
            add_new_possible_cells_to_go.Clear();
            if (new_possible_cells_to_go.Count == 0 || new_possible_cells_to_go.Contains((2, 2)))
            {
                found_path = true;
            }
        }
        //find path cells out of possible path cells
        while (true)
        {
            player_x = path_cells[path_cells.Count-1].Item1;
            player_y = path_cells[path_cells.Count-1].Item2;
            if (player_x != 4)
            {
                if (possible_cells_to_go.Contains((player_x+1, player_y)))
                {
                    next_move_cells.Add((player_x+1, player_y));
                }
            }
            if (player_x != 0)
            {
                if (possible_cells_to_go.Contains((player_x-1, player_y)))
                {
                    next_move_cells.Add((player_x-1, player_y));
                }
            }
            if (player_y != 4)
            {
                if (possible_cells_to_go.Contains((player_x, player_y+1)))
                {
                    next_move_cells.Add((player_x, player_y+1));
                }
            }
            if (player_y != 0)
            {
                if (possible_cells_to_go.Contains((player_x, player_y-1)))
                {
                    next_move_cells.Add((player_x, player_y-1));
                }
            }
            int shortest_distance = 50;
            int index = 0;
            for (int i = 0; i < next_move_cells.Count; i++)
            {
                int cell_x = next_move_cells[i].Item1;
                int cell_y = next_move_cells[i].Item2;
                int distance_cell_item = (item_x - cell_x)*(item_x - cell_x) + (item_y - cell_y)*(item_y - cell_y);
                if (shortest_distance > distance_cell_item)
                {
                    index = i;
                    shortest_distance = distance_cell_item;
                }
            }   
            path_cells.Add(next_move_cells[index]);
            next_move_cells.Clear();
            if (path_cells[path_cells.Count-1] == (item_x, item_y))
            {
                break;
            }
            
        } 
        //convert path cells into direction list
        for (int i = 0; i < path_cells.Count-1; i++)
        {
            int delta_x = path_cells[i+1].Item1 - path_cells[i].Item1;
            int delta_y = path_cells[i+1].Item2 - path_cells[i].Item2;
            if (delta_x == 1)
            {
                path.Add(2);
            }
            else if (delta_x == -1)
            {
                path.Add(4);
            }
            else if (delta_y == 1)
            {
                path.Add(3);
            }
            else if (delta_y == -1)
            {
                path.Add(1);
            }
        }
        Console.WriteLine(path.Count);
        return path;

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