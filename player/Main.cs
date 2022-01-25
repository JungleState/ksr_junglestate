using CommandLine;
using Microsoft.Extensions.Logging;

namespace junglestate;

class GlobalOptions {
    [Option('s', "server", Required = false, HelpText = "Server URL.", Default = "https://junglecamp-tfszpy3t2a-oa.a.run.app/")]
    public string Server { get; set; } = "https://junglecamp-tfszpy3t2a-oa.a.run.app/";
    [Option('m', "monkey", Required = false, HelpText = "The monkey class to use.", Default = "")]
    public string Monkey { get; set; } = "";
    [Option('d', "delay", Required = false, HelpText = "Update delay in millis.", Default = 100)]
    public int Delay { get; set; } = 500;
    [Option('n', "name", Required = false, HelpText = "The monkey name, must be unique per game.", Default = "Hooey")]
    public string Name { get; set; } = "Hooey";
    [Option('q', "quiet", Required = false, HelpText = "Do not show game behavior in console.", Default = false)]
    public bool Quiet { get; set; } = false;
    [Option('t', "time", Required = false, HelpText = "Show average latency of HTTP calls.", Default = false)]
    public bool Time { get; set; } = false;
    [Option('p', "password", Required = false, HelpText = "Game password.", Default = "")]
    public string Password { get; set; } = "";
}

[Verb("ask", isDefault: true, HelpText = "Ask for options.")]
class AskOptions : GlobalOptions {
}

[Verb("join", HelpText = "Join an existing game.")]
class JoinOptions : GlobalOptions {
    [Option('g', "game", Required = true, HelpText = "Game id.", Default = "")]
    public string GameId { get; set; } = "";
    [Option('P', "performance", Required = false, HelpText = "Start that many monkeys in parallel.", Default = 1)]
    public int PerformanceCount { get; set; } = 1;
}
[Verb("start", HelpText = "Start a new game.")]
class StartOptions : GlobalOptions {
    //start options here
}

class MonkeyCommandLine {
    public static async Task Main(string[] args) {
        MonkeyCommandLine cli = new MonkeyCommandLine();
        try {
            await Parser.Default.ParseArguments<JoinOptions, StartOptions, AskOptions>(args)
                    .MapResult(
                        (JoinOptions joinOpts) => cli.JoinMain(joinOpts),
                        (StartOptions startOpts) => cli.StartMain(startOpts),
                        (AskOptions askOpts) => cli.AskMain(askOpts),
                        errs => Task.FromResult(1)
                    );
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }

    private async Task joinGame(JungleConnection connection, JungleConfig config, string gameId) {
        await connection.joinGame(gameId);
    }

    private async Task JoinMain(JoinOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, false);
        JungleConfig config = readGlobalOptions(options);
        if (options.PerformanceCount > 1) {
            List<Task> connections = new List<Task>();
            for (int i = 0; i < options.PerformanceCount; i++) {
                monkey.Name = options.Name + $"_{i}";
                JungleConnection connection = new JungleConnection(monkey, config);
                connections.Add(joinGame(connection, config, options.GameId));
            }
            Task.WaitAll(connections.ToArray());
        } else {
            monkey.Name = options.Name;
            using JungleConnection connection = new JungleConnection(monkey, config);
            await joinGame(connection, config, options.GameId);
        }
    }

    private async Task StartMain(StartOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, false);
        JungleConfig config = readGlobalOptions(options);
        config.serverAddress = new Uri(options.Server);
        monkey.Name = options.Name;
        using JungleConnection connection = new JungleConnection(monkey, config);
        await joinGame(connection, config, "");
    }

    private JungleConfig readGlobalOptions(GlobalOptions options) {
        JungleConfig config = new JungleConfig();
        config.delay_ms = options.Delay;
        config.serverAddress = new Uri(options.Server);
        config.useConsole = !options.Quiet;
        config.password = options.Password;
        config.showTimers = options.Time;
        using var loggerFactory = LoggerFactory.Create(builder => {
            builder
                .AddSimpleConsole(options => {
                    options.SingleLine = true;
                });
        });
        config.logger = loggerFactory.CreateLogger<JungleConnection>();
        return config;
    }

    private async Task AskMain(AskOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, true);
        JungleConfig config = readGlobalOptions(options);
        Console.WriteLine($"Select server (default: {options.Server}): ");
        string? server = Console.ReadLine();
        if (String.IsNullOrEmpty(server)) {
            server = options.Server;
        }
        config.serverAddress = new Uri(server);

        // Console.WriteLine($"Update delay in millis (default: {options.Delay}): ");
        // string? delay = Console.ReadLine();
        // if (!String.IsNullOrEmpty(delay)) {
        //     int delayVal = options.Delay;
        //     int.TryParse(delay, out delayVal);
        //     options.Delay = delayVal;
        // }
        // config.delay_ms = options.Delay;
        config.delay_ms = 100;

        Console.WriteLine($"Monkey name (default: {options.Name}): ");
        string? name = Console.ReadLine();
        if (String.IsNullOrEmpty(name)) {
            name = options.Name;
        }
        monkey.Name = name;

        using JungleConnection connection = new JungleConnection(monkey, config);
        dynamic games = await connection.listGames();
        Console.WriteLine("Start your monkey as follows: ");
        Console.WriteLine("Start new game (0) (default)");
        int key = 0;
        foreach (dynamic game in games) {
            key++;
            Console.WriteLine($"Join game '{game.id}' ({key})");
        }
        string? input = Console.ReadLine();
        int selection;
        string gameId = "";
        if (int.TryParse(input, out selection) && selection > 0) {
            gameId = games[selection - 1].id;
        }

        await joinGame(connection, config, gameId);
    }

    private static BaseMonkey instantiateMonkey(GlobalOptions options, bool allowAsking) {
        var monkeyClasses =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(BaseMonkey))
            select type;

        int monkeyCandidateCount = monkeyClasses.Count();
        Type? monkeyType = null;
        if (monkeyCandidateCount == 0) {
            throw new Exception("No classes extending BaseMonkey found.");
        } else if (monkeyCandidateCount == 1) {
            monkeyType = monkeyClasses.Single();
        } else if (!allowAsking) {
            if (String.IsNullOrEmpty(options.Monkey)) {
                throw new Exception("multiple possible monkeys, select one with --monkey or use 'ask'");
            }
            foreach (Type type in monkeyClasses) {
                if (type.Name == options.Monkey) {
                    monkeyType = type;
                    break;
                }
            }
            if (monkeyType == null) {
                throw new Exception("No matching monkey class found");
            }
        } else {
            // multiple monkey class candidates
            int selection = 0;
            int key = 0;
            monkeyType = monkeyClasses.First();
            foreach (Type type in monkeyClasses) {
                Console.WriteLine($"{key} Use class '{type.Name}'");
                if (type.Name == options.Monkey) {
                    monkeyType = type;
                    selection = key;
                }
                key++;
            }
            Console.Write($"Select monkey class [{selection} - {monkeyType.Name}]): ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out selection) && selection >= 0 && selection < monkeyCandidateCount) {
                monkeyType = monkeyClasses.ElementAt(selection);
            }
        }
        Console.WriteLine($"Using monkey class {monkeyType.Name}");
        BaseMonkey? monkey = (BaseMonkey?)Activator.CreateInstance(monkeyType);
        if (monkey == null) {
            throw new Exception($"Unable to instantiate monkey class {monkeyType}");
        }
        return monkey;
    }
}
