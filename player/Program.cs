using Newtonsoft.Json;

// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Download Json.NET: "dotnet add package Newtonsoft.Json
//     - Run: "dotnet run"

namespace Player {
    class Program {
        private static readonly HttpClient client = new HttpClient();

        private static async Task joinGame(string name) {
            var stringTask = client.GetStringAsync("http://localhost:5500/joinGame/client/"+name);
            var json = await stringTask;
            dynamic data = JsonConvert.DeserializeObject(json);

            // Console.WriteLine(json);

            if (data.ok == false) {
                Console.WriteLine("Connection to game failed!");
                Console.WriteLine("Your name is already being used. Change name and try again.");
            }
        }
        private static async Task getData() {
            var stringTask = client.GetStringAsync("http://localhost:5500/view");
            var json = await stringTask;
            dynamic data = JsonConvert.DeserializeObject(json);

            Console.WriteLine(json);

            playerBehaviour(data.field);
        }

        private static void move(int direction) {
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand(1, direction);
            }
            else {
                sendCommand(0, -1);
            }
        }

        private static void attack(int direction) {
            if (direction >= 0 && direction <= 7) {
                sendCommand(2, direction);
            }
            else {
                sendCommand(0, -1);
            }
        }

        private static async void sendCommand(int type, int direction) {
            var stringTask = client.GetStringAsync("http://localhost:5500/action/"+type+"/"+direction);
            var json = await stringTask;
        }

        private static string configs() {
            /////////YOUR/CONFIGS/BELOW/HERE//////////

            string name = "Hans Muster";

            //////////YOUR/CONFIGS/ABOVE/HERE//////////
            return name;
        }
        private static void playerBehaviour(string field) {        
            // attack:  attack(DIRECTION)   options for DIRECTION: [0, 7]
            // move:    move(DIRECTION)     options for DIRECTION: -1, 0, 2, 4, 6
            // DIRECTION: integer
            //    -1: No direction
            //    0: up
            //    1: up right
            //    2: right
            //    3: down right
            //    4: down
            //    5: down left
            //    6: left
            //    7: up left

            // string field: 5x5 matrix of surrounding
                
            //////////YOUR/CODE/BELOW/HERE//////////

            move(-1);

            //////////YOUR/CODE/ABOVE/HERE//////////
        }

        static async Task Main(string[] args) {
            await getData();
            // await joinGame(configs());










        }
    }
}
