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

        private static async Task joinGame() {
            
        }
        private static async Task getData() {
            // get the json (strings)
            var stringTask = client.GetStringAsync("http://localhost:5000/view");
            var json = await stringTask;

            Console.WriteLine(json);

            // get data from string
        }


        private static void playerBehaviour() {            
            //////////YOUR/CODE/BELOW/HERE//////////
            



            //////////YOUR/CODE/ABOVE/HERE//////////
        }

        static async Task Main(string[] args) {
            await getData();
        }
    }
}
