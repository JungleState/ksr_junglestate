using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace junglestate;
sealed class JungleConfig {
    internal Uri serverAddress = new Uri("http://localhost:5500/");
    internal string password = "";
    internal int delay_ms = 500;
    internal bool useConsole = true;
    internal bool showTimers = false;
    internal ILogger logger = NullLogger.Instance;
}

///<summary>The main program for junglecamp monkey bots.</summary>
///<remarks>Once a program is started, it maintains a HTTP connection to the game server,
///  listens for state updates and calls the monkey's <see cref="BaseMonkey.nextMove(GameState)"/>
///  method determine the monkey's behavior.
///</remarks>
class JungleConnection : IDisposable {
    private readonly HttpClient client;
    private readonly JungleConfig config;
    private readonly BaseMonkey monkey;

    private int lastRound = -1;

    public readonly TaskMeasurer viewMeasurer = new TaskMeasurer("view");
    public readonly TaskMeasurer actionMeasurer = new TaskMeasurer("action");

    internal JungleConnection(BaseMonkey monkey, JungleConfig config) {
        this.config = config;
        this.monkey = monkey;
        this.client = new HttpClient();
    }

    public void Dispose() {
        client.Dispose();
    }

    ///<summary>Lists the games running on the server, possibly none.</summary>
    internal async Task<dynamic> listGames() {
        var stringTask = client.GetStringAsync(new Uri(config.serverAddress, "getGames"));
        var json = await stringTask;
        dynamic? data = JsonConvert.DeserializeObject(json);
        if (data == null) {
            throw new Exception("No games");
        }

        return data.games;
    }

    ///<summary>Joins the given game, or starts a new if gameId is empty.</summary>
    internal async Task joinGame(string gameId) {
        // join game (fetch request)
        var joinData = new {
            player_name = monkey.Name,
            player_mode = "client",
            password = config.password,
            mode = String.IsNullOrEmpty(gameId) ? "newGame" : "joinExisting",
            game_id = gameId
        };
        string jsonData = JsonConvert.SerializeObject(joinData);
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(new Uri(config.serverAddress, "joinGame"), content);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) {
            config.logger.LogError(json);
            return;
        }
        dynamic? data = JsonConvert.DeserializeObject(json);

        // check if joining was successful
        if (data == null) {
            Console.WriteLine("ERROR: data is null");
            return;
        }
        if (data.ok == false) {
            config.logger.LogError($"Connection to game failed! {data.msg}");
            return;
        }
        int score = 0;
        // loop with regular request for the field followed by an instruction for the server what to do
        while (true) {
            Tuple<bool, int> returnedData = await getData();
            Thread.Sleep(config.delay_ms);
            if (returnedData.Item1) {
                score = returnedData.Item2;
                break;
            }
        }
        if (config.useConsole) {
            Console.WriteLine("");
            Console.WriteLine("!!!!!!!!!!!");
            Console.WriteLine("!GAME OVER!");
            Console.WriteLine("!!!!!!!!!!!");
            Console.WriteLine("Score: " + score);
        }
    }

    private async Task<Tuple<bool, int>> getData() {
        // get the map
        var json = await viewMeasurer.measureTask(client.GetStringAsync(new Uri(config.serverAddress, "view")));
        dynamic? data = JsonConvert.DeserializeObject(json);

        bool gameOver = false;
        int score = 0;

        if (data == null) {
            config.logger.LogError("ERROR: view data is null");
        } else {
            GameState state = parseGameState(data);
            if (data.lives > 0) {
                if (data.round != lastRound) {
                    lastRound = data.round;
                    printState(data);
                    playerBehaviour(state);
                    logTimers();
                } else {
                    config.logger.LogWarning("no update, same round");
                }
            } else {
                // end loop from joinGame
                gameOver = true;
            }
        }
        return new Tuple<bool, int>(gameOver, score);
    }

    private void printState(dynamic data) {
        if (config.useConsole) {
            Console.Clear();
            Console.WriteLine("Field: " + data.field);
            Console.WriteLine("Round: " + data.round);
            Console.WriteLine("Health: " + data.lives);
            Console.WriteLine("Ammo: " + data.coconuts);
            Console.WriteLine("Score: " + data.points);
        }
    }

    private void logTimers() {
        if (config.showTimers) {
            config.logger.LogInformation($"View: {viewMeasurer.average().TotalMilliseconds}ms");
            config.logger.LogInformation($"Action: {actionMeasurer.average().TotalMilliseconds}ms");
        }
    }

    private void playerBehaviour(GameState state) {
        Move next = monkey.nextMove(state);
        switch (next.action) {
            case Action.MOVE:
                if (next.direction.isMoveable()) {
                    sendCommand(next.action, next.direction);
                }
                break;
            case Action.THROW:
                sendCommand(next.action, next.direction);
                break;
            default:
                sendCommand(Action.STAY, Direction.NONE);
                break;
        }
    }

    private async void sendCommand(Action action, Direction direction) {
        try {
            if (config.useConsole) {
                Console.WriteLine("-> Action: " + action.ToString() + " " + direction.ToString());
            }
            await actionMeasurer.measureTask(client.PostAsync(new Uri(config.serverAddress, "action/" + (int)action + "/" + (int)direction), new StringContent("")));
        } catch (Exception e) {
            config.logger.LogError(e, "Problem sending command.");
        }
    }

    private GameState parseGameState(dynamic data) {
        return new GameState(getCells(data.field.ToObject<string[]>()),
                                data.round.ToObject<int>(),
                                new PlayerInfo(monkey.Name,
                                    data.lives.ToObject<int>(),
                                    data.coconuts.ToObject<int>(),
                                    data.points.ToObject<int>()));
    }

    private static Cell[][] getCells(string[] field) {
        Cell[][] cells = new Cell[5][];
        int rowIndex = 0;
        foreach (string row in field) {
            cells[rowIndex] = new Cell[5];
            for (int i = 0; i < row.Length / 2; i++) {
                string cellString = row.Substring(i * 2, 2);
                cells[rowIndex][i] = getCell(cellString);
            }
            rowIndex++;
        }
        return cells;
    }

    private static Cell getCell(string cellString) {
        Item item = ItemInfo.fromCode(cellString);
        if (item == Item.PLAYER) {
            // TODO create player info from json response.
            PlayerInfo info = new PlayerInfo("player", 3, 2, 25);
            return Cell.PlayerCell(info);
        }
        return Cell.ItemCell(item);
    }
}

public sealed class TaskMeasurer {
    public TimeSpan total {get; private set;} = TimeSpan.Zero;
    public long count {get; private set;} = 0;

    public readonly string operation;

    public TaskMeasurer(string operation) {
        this.operation = operation;
    }

    public async Task<T> measureTask<T>(Task<T> task) {
        Stopwatch stopWatch = Stopwatch.StartNew();
        count++;
        T result = await task;
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        total = total + stopWatch.Elapsed;
        return result;
    }

    public TimeSpan average() {
        if (count == 0) {
            return TimeSpan.Zero;
        }
        return total / count;
    }
}
