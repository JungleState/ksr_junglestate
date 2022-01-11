# JungleState Documentation

## Spectator mode
* Enter server name into web browser (e.g. https://junglecamp-tfszpy3t2a-oa.a.run.app)
* Enter Nickname
* Tick "Set as Spectator"
* Click "New Game" OR
* Join existing server (press "Join")

## Manual control
* Enter server name into web browser (e.g. https://junglecamp-tfszpy3t2a-oa.a.run.app)
* Enter Nickname
* Start new game (click "New Game") OR
* Join existing server (click "Join")
* Controls:
    * Move: "W", "A", "S", "D"
    * Attack: "U", "I", "O", "J", "K"/"L", "N", "M", ","

## Requirements for bot application (in Visual Studio Code)
* C# Extension (Ctrl+Shift+X, "C#")
* .NET (Download here: https://dotnet.microsoft.com/download)
* Json.NET (open terminal (Ctrl+Shift+^), enter: "dotnet add package Newtonsoft.Json")

## Start bot application
* Open terminal (Ctrl+Shift+^)
* Change directory to "/ksr_junglestate/player" (enter "cd player")
* Join Game: OR
    * Enter: "dotnet run ask"
* Join Game with configs in command line: OR
    * Enter: "dotnet run -- join --game [GAME_NAME]  --name [PLAYER_NAME] --server [SERVER_NAME]"
    * Replace "[GAME_NAME]" with the game name (e.g. "f91938db-c6d0-496c-940f-98c22f62ad87")
    * Replace "[PLAYER_NAME]" with the player name (e.g. "Booey")
    * Replace "[SERVER_NAME]" with the server name (e.g. "https://junglecamp-tfszpy3t2a-oa.a.run.app")
* Start new server and start:
    * Enter: "dotnet run -- start --name [PLAYER_NAME] --server [SERVER_NAME]"
    * Replace "[PLAYER_NAME]" with the player name (e.g. "Booey")
    * Replace "[SERVER_NAME]" with the server name (e.g. "https://junglecamp-tfszpy3t2a-oa.a.run.app")

## Monkey bot code
Every turn the method "nextMove(GameState state)" executed. An object of the type "Move" needs to be returned.

### Gamestate
Gamestate types have following attributes:
* Cell[][] cells: 5x5 array of cells visible around the monkey's position.
* int round: game round identifier
* PlayerInfo playerInfo: monkey's own player information
* getCell(Direction dir): return


* PlayerInfo types have following attributes:
    * string name: name of the player
    * int lives: lives remaining for the player
    * int coconuts: number of coconuts remianing for the player
    * int points: number of points scored by the player


## Bot usage hints
* When terminal stops showing inputs:
    * Enter: "reset" OR
    * Just ignore, it works anyway