# JungleState Documentation
This documentation explains how to use JungleState and how to create your own monkey bot. All shortcuts given are based on default settings and may differ.

## Spectator mode
* Enter server name into web browser (e.g. https://junglecamp-tfszpy3t2a-oa.a.run.app)
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

## Requirements for bot application
* Following instructions are based on Visual Studio Code. Other IDEs may work as well but aren't tested. (Download here (choose option based on your OS): https://code.visualstudio.com/Download OR "sudo apt install code" (may differ depending on package manager))
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
Edit the code in Monkey.cs to create your
Every turn the method "nextMove(GameState)" executed. An object of the type "Move" needs to be returned.

### Gamestate
Gamestate types have following members:
* Cell[][] cells: 5x5 array of cells visible around the monkey's position.
* int round: game round identifier
* PlayerInfo playerInfo: monkey's own player information
* getCell(Direction): returns Cell at the given direction

### Move
Move types have following members:
* Action action: action to be executed
* Direction direction: direction of the action
* Optional:
    * string message: message displayed to spectators during round
    * int nextRound: round identifier that allows the game manager to detect out-of-sync moves

### Action
Following actions exist:
* Action.STAY: stay on the spot, direction is ignored
* Action.MOVE: move into given direction, diagonal is NOT possible
* Action.THROW: throw coconut at given direction, diagonal is possible, range: one field

### Direction
Following directions exist:
* Direction.NONE
* Direction.UP
* Direction.UP_RIGHT
* Direction.RIGHT
* Direction.DOWN_RIGHT
* Direction.DOWN
* Direction.DOWN_LEFT
* Direction.LEFT
* Direction.UP_LEFT

### Item
Following items exist:
* Item.EMPTY
* Item.FOREST
* Item.BANANA
* Item.COCONUT
* Item.PINEAPPLE
* Item.PLAYER

### DirectionInfo
Methods that can be attached to directions are:
* Coordinates(): returnes the coordinates (from the 5x5 array) of the direction as a Tuple<int, int>
* isMoveable(): returnes if player can move into given direction as bool (does NOT check if player would take damage etc.)
* opposite(): gives the opposite direction as Direction

### ItemInfo
Methods that can be attached to items are:
* isMoveable(): returns if player can move to a field containing given item as a bool

### PlayerInfo
PlayerInfo types have following members:
* string name: name of the player
* int lives: lives remaining for the player
* int coconuts: number of coconuts remianing for the player
* int points: number of points scored by the player

### Cell
Cell types have following members:
* isFree(): returns if player can move to as a bool
* Item item: kind of item contained in cell
<!-- * PlayerInfo playerinfo: optional info about player in cell  -->

## Bot usage hints
* When terminal stops showing inputs:
    * Enter: "reset" OR
    * Just ignore, it works anyway
