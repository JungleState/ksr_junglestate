using CommandLine;
using Microsoft.Extensions.Logging;

namespace junglestate;

class GlobalOptions {
    [Option('s', "server", Required = false, HelpText = "Server URL.", Default = "http://localhost:5500/")]
    public string Server { get; set; } = "http://localhost:5500/";
    [Option('m', "monkey", Required = false, HelpText = "The monkey class to use.", Default = "")]
    public string Monkey { get; set; } = "";
    [Option('d', "delay", Required = false, HelpText = "Update delay in millis.", Default = 500)]
    public int Delay { get; set; } = 500;
    [Option('n', "name", Required = false, HelpText = "The monkey name, must be unique per game.", Default = "Hooey")]
    public string Name { get; set; } = "Hooey";
    [Option('c', "console", Required = false, HelpText = "Output responses and actions to console.", Default = true)]
    public bool Console { get; set; } = true;
}

[Verb("ask", isDefault: true, HelpText = "Ask for options.")]
class AskOptions : GlobalOptions {
}

[Verb("join", HelpText = "Join an existing game.")]
class JoinOptions : GlobalOptions {
    [Option('g', "game", Required = true, HelpText = "Game id.", Default = "")]
    public string GameId { get; set; } = "";
    [Option('p', "password", Required = false, HelpText = "Game password.", Default = "")]
    public string Password { get; set; } = "";
}
[Verb("start", HelpText = "Start a new game.")]
class StartOptions : GlobalOptions {
    //start options here
}

class MonkeyCommandLine {
    public static async Task Main(string[] args) {
        using var loggerFactory = LoggerFactory.Create(builder => {
            builder
                .AddSimpleConsole(options => {
                    options.SingleLine = true;
                });
        });

        JungleConfig config = new JungleConfig();
        config.logger = loggerFactory.CreateLogger<Program>();
        try {
            await Parser.Default.ParseArguments<JoinOptions, StartOptions, AskOptions>(args)
                    .MapResult(
                        (JoinOptions joinOpts) => JoinMain(joinOpts),
                        (StartOptions startOpts) => StartMain(startOpts),
                        (AskOptions askOpts) => AskMain(askOpts),
                        errs => Task.FromResult(1)
                    );
        } catch (Exception e) {
            config.logger.LogWarning(e.Message);
            config.logger.LogDebug(e, "Error running program");
        }
    }

    private static async Task JoinMain(JoinOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, false);
        JungleConfig config = new JungleConfig();
        readGlobalOptions(options, config);
        config.password = options.Password;
        monkey.Name = options.Name;
        Program program = new Program(monkey, config);
        await program.joinGame(options.GameId);
    }

    private static async Task StartMain(StartOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, false);
        JungleConfig config = new JungleConfig();
        readGlobalOptions(options, config);
        config.serverAddress = new Uri(options.Server);
        monkey.Name = options.Name;
        Program program = new Program(monkey, config);
        await program.joinGame("");
    }

    private static void readGlobalOptions(GlobalOptions options, JungleConfig config) {
        config.delay_ms = options.Delay;
        config.serverAddress = new Uri(options.Server);
        config.useConsole = options.Console;
    }

    private static async Task AskMain(AskOptions options) {
        BaseMonkey monkey = instantiateMonkey(options, true);
        JungleConfig config = new JungleConfig();
        readGlobalOptions(options, config);
        Console.WriteLine($"Select server (default: {options.Server}): ");
        string? server = Console.ReadLine();
        if (String.IsNullOrEmpty(server)) {
            server = options.Server;
        }
        config.serverAddress = new Uri(server);

        Console.WriteLine($"Update delay in millis (default: {options.Delay}): ");
        string? delay = Console.ReadLine();
        if (!String.IsNullOrEmpty(delay)) {
            int delayVal = options.Delay;
            int.TryParse(delay, out delayVal);
            options.Delay = delayVal;
        }
        config.delay_ms = options.Delay;

        Console.WriteLine($"Monkey name (default: {options.Name}): ");
        string? name = Console.ReadLine();
        if (String.IsNullOrEmpty(name)) {
            name = options.Name;
        }
        monkey.Name = name;

        Program program = new Program(monkey, config);
        dynamic games = await program.listGames();
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

        await program.joinGame(gameId);
    }

    private static BaseMonkey instantiateMonkey(GlobalOptions options, bool allowAsking) {
        var monkeyClasses =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(BaseMonkey))
            select type;

        int monkeyCandidateCount = monkeyClasses.Count();
        Type monkeyType;
        if (monkeyCandidateCount == 0) {
            throw new Exception("No classes extending BaseMonkey found.");
        } else if (monkeyCandidateCount == 1) {
            monkeyType = monkeyClasses.Single();
        } else {
            if (!allowAsking) {
                throw new Exception("multiple possible monkeys, select one with --monkey or use 'ask'");
            }
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