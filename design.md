# Design

## Components

### Server

* app.py: Wie in Vier-Gewinnt. 
* gameLogic.py: Beinhalted game logic
* collector.py: Sammelt eingehende Aktionen, führt Sicherheitscheck durch

### Client

* client.cs: Vorlage, mit Erweiterungsmöglichkeit
* manualController.cs: Manuelle Steuerung über Terminal

### Visualization

* view.html: Zeigt komplettes Spiel
* styleSheet.css: Verschönert view.html
* script.js: Anzeigelogik

## Implementation of REST-API

Website (Rückgabe in HTML)
* /: Gibt view Website

Spielsuche (Rückgabe in JSON):
* /joinGame: POST; Tritt Spiel bei, wenn keins vorhanden: erstellt eins

Aktives Spiel (Rückgabe in JSON):
* /spectator/view/\<gameID>: GET; Gibt Spielmatrix zurück

Client endpoints (Rückgabe in JSON):
* /player/action/\<gameID>/\<playerID>/\<Action>: POST; Aktion an Server
* /player/getEnvironment/\<gameID>/\<playerID>: GET; Gibt 5x5 Matrix um den Spieler zurück



## JSON Structure

Spectator:
* GameID
* gameField (Matrix etwa 50x50)
* gameState (win or running)
* round (Welche Runde?)
* playerList (Liste der Spieler, jeder Spieler hat eine eigene Liste mit:)
    * id
    * name
    * health
    * coconuts 
    * bananas
    * pinapples
    * points

Client:
* fieldMatrix (5x5 Matrix um ihn herum)
* items
    * coconuts
    * bananas
    * pinapples
* round




