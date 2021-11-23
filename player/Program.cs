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

        }

        private static string config() {
            /////////YOUR/CODE/BELOW/HERE//////////
            string name = "Hans Muster";
            //////////YOUR/CODE/ABOVE/HERE//////////
            return name;
        }
        private static void playerBehaviour() {            
            //////////YOUR/CODE/BELOW/HERE//////////
            // attack:  attack(DIRECTION)
            // move:    move(DIRECTION)

            // DIRECTION: integer
            // 1 2 3
            // 
            



            //////////YOUR/CODE/ABOVE/HERE//////////
        }

        static async Task Main(string[] args) {
            // await getData();
            await joinGame();
        }
    }
}
