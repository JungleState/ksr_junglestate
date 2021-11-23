using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

// For usage in Visual Studio Code:
//     - Download C# extension
//     - Download NET: https://dotnet.microsoft.com/download
//     - Open Terminal
//     - Move into "player" folder: "cd player"
//     - Run: "dotnet run"

namespace Player {
    class Program {
        private static readonly HttpClient client = new HttpClient();

        private static async Task joinGame(string name) {
            var stringTask = client.GetStringAsync("http://localhost:5500/joinGame/client/"+name);
            var json = await stringTask;

            Console.WriteLine(json);
        }
        private static async Task getData() {
            var stringTask = client.GetStringAsync("http://localhost:5500/view");
            var json = await stringTask;

            Console.WriteLine(json);


        }

        private static void move(int direction) {
            
        }

        private static void attack(int direction) {
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand("attack", direction);
            }
            else {
                sendCommand("")
            }
        }

        private static void sendCommand(string move, int direction) {

        }

        private static string config() {
            /////////YOUR/CONFIGS/BELOW/HERE//////////

            string name = "Hans Muster";

            //////////YOUR/CONFIGS/ABOVE/HERE//////////
            return name;
        }
        private static void playerBehaviour() {        
            // move:    move(DIRECTION)     options for DIRECTION: N [-1, 7]
            // attack:  attack(DIRECTION)   options for DIRECTION: 0, 2, 4, 6
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
                
            //////////YOUR/CODE/BELOW/HERE//////////

            move(-1);

            //////////YOUR/CODE/ABOVE/HERE//////////
        }

        static async Task Main(string[] args) {
            // await getData();
            // await joinGame(config());





        }
    }
}
