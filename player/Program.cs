﻿using Newtonsoft.Json;

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

        private static async Task joinGame(Tuple<string, string> configs) {
            // join game (fetch request)
            var stringTask = client.GetStringAsync(configs.Item1+"joinGame/client/"+configs.Item2);
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
                        await getData(configs);
                        Thread.Sleep(500);
                    }
                    Console.WriteLine("!!!!!!!!!!!");
                    Console.WriteLine("!GAME OVER!");
                    Console.WriteLine("!!!!!!!!!!!");
                }   
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
        }
        private static async Task getData(Tuple<string, string> configs) {
            // get the map
            var stringTask = client.GetStringAsync(configs.Item1+"view");
            var json = await stringTask;
            dynamic? data = JsonConvert.DeserializeObject(json);

            if (data != null) {
                // ask user written algorithm about what to do
                if (data.lives > 0) {
                    playerBehaviour(configs, data.field, data.lives, data.coconuts, data.points, data.round);
                }
                else {
                    // end loop from joinGame






                }
            }
            else {
                Console.WriteLine("ERROR: data is null");
            }
        }

        private static void move(Tuple<string, string> configs, int direction) {
            // option for user written algorithm
            // direction defines the direction the player moves
            if (direction == 0 || direction == 2 || direction == 4 || direction == 6) {
                sendCommand(configs, 1, direction);
            }
            // if an invalid direction is given or -1 is stated the character does nothing
            else {
                sendCommand(configs, 0, -1);
            }
        }

        private static void attack(Tuple<string, string> configs, int direction) {
            // option for user written algorithm
            // direction defines the direction the player shoots
            if (direction >= 0 && direction <= 7) {
                sendCommand(configs, 2, direction);
            }
            // if an invalid direction is given the character does nothing
            else {
                sendCommand(configs, 0, -1);
            }
        }

        private static async void sendCommand(Tuple<string, string> configs, int type, int direction) {
            // send the chosen action to the server
            var stringTask = client.GetStringAsync(configs.Item1+"action/"+type+"/"+direction);
            var json = await stringTask;
        }

        private static Tuple<string, string> loadConfigs() {
            // configurations that need to be changed by the user

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/CONFIGS/BELOW/HERE//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//





// url for server
// e.g. http://localhost:5500/
string url = "http://localhost:5500/";

// name of player
// e.g. Max Mustermann
string name = "Max Mustermann";





//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/CONFIGS/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
            Tuple<string, string> configs = new Tuple<string, string>(url, name);
            return configs;
        }

        private static void playerBehaviour(Tuple<string, string> c, string field, int health, int ammo, int score, int round) {   
        // method with the user written algorithm
                
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/CODE/BELOW/HERE/vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//

// attack:  attack(c, DIRECTION)   options for DIRECTION: [0, 7]
// move:    move(c, DIRECTION)     options for DIRECTION: -1, 0, 2, 4, 6
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
// Tuple<string, string> c: configurations, can be ignored but must be given as an argument for attack() and move()

// custom mehtods can be added in the next section.
// DON'T DO IT HERE!





Random random = new Random();
int decision = random.Next(0, 2);

if (decision == 0){
    move(c, -1);
}
else {
    moveLeft(c);
}





//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/CODE/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

        }

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv/YOUR/METHODS/BELOW/HERE//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//

// space for some custom methods
// if attack() or move() methods are used, you need to give the "c" as an argument
// e.g. private static void moveLeft(Tuple<string, string> c) {...}





private static void moveLeft(Tuple<string, string> c) {
    move(c, 6);
}





//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^/YOUR/METHODS/ABOVE/HERE/^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//



        static async Task Main(string[] args) {
            // start the process
            await joinGame(loadConfigs());
        }
    }
}
