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
            // join game (fetch request)
            var stringTask = client.GetStringAsync("http://localhost:5500/joinGame/client/"+name);
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);

            // check if joining was successful
            if (data != null){
                if (data.ok == false) {
                Console.WriteLine("Connection to game failed!");
                Console.WriteLine("Problem assumption: Your name is already being used. Change name and try again.");
                }
                else {
                    // loop with regular request for the field followed by an instruction for the server what to do
                    bool running = true;
                    while (running) {
                        await getData();
                        Thread.Sleep(500);
                    }
                }   
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
        }
        private static async Task getData() {
            // get the map
            var stringTask = client.GetStringAsync("http://localhost:5500/view");
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);

            if (data != null) {
                // ask user written algorithm about what to do
                playerBehaviour(data.field, data.lives, data.coconuts, data.points, data.round);
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
        }

        private static void move(int direction) {
            // option for user written algorithm
            // direction defines the direction the player moves
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand(1, direction);
            }
            // if an invalid direction is given or -1 is stated the character does nothing
            else {
                sendCommand(0, -1);
            }
        }

        private static void attack(int direction) {
            // option for user written algorithm
            // direction defines the direction the player shoots
            if (direction >= 0 && direction <= 7) {
                sendCommand(2, direction);
            }
            // if an invalid direction is given the character does nothing
            else {
                sendCommand(0, -1);
            }
        }

        private static async void sendCommand(int type, int direction) {
            // send the chosen action to the server
            var stringTask = client.GetStringAsync("http://localhost:5500/action/"+type+"/"+direction);
            var json = await stringTask;
        }

        private static string configs() {
            // configurations that need to be changed by the user

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/CONFIGS/BELOW/HERE//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//





string name = "Max Mustermann";




//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/CONFIGS/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

            return name;
        }

        private static void playerBehaviour(string field, int health, int ammo, int score, int round) {   
        // method with the user written algorithm
                
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/CODE/BELOW/HERE/VVVVvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//

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

// "  ": plain:     empty
// "FF": jungle:    wall
// "CC": coconut:   +1 ammo
// "BB": banana:    +1 health
// "PP": pineapple: +1 score

// string field: 5x5 matrix of surrounding
// int health: health of character
// int ammo: amount of ammo left
// int score: score of player
// int round: round of the game

// custom mehtods can be added in the next section.
// DON'T DO IT HERE!





Random random = new Random();
int decision = random.Next(0, 2);

if (decision == 0){
    move(-1);
}
else {
    moveLeft();
}





//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/CODE/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

        }

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/METHODS/BELOW/HERE//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//

// space for some custom methods





private static void moveLeft() {
    move(6);
}





//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/METHODS/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//



        static async Task Main(string[] args) {
            // start the process
            await joinGame(configs());
        }
    }
}
