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
* /SpectView/\<gameID>: GET; Gibt Spielmatrix zurück

Client endpoints (Rückgabe in JSON):
* /player/action/\<gameID>/\<playerID>/\<Action>: POST; Aktion an Server
* /player/getEnvironment/\<gameID>/\<playerID>: GET; Gibt 5x5 Matrix um den Spieler zurück



## JSON Structure

Spectator:
* id
* field (Matrix etwa 50x50)
* state (win or running)
* round (Welche Runde?)
* player_list (Liste der Spieler, jeder Spieler hat eine eigene Liste mit:)
    * id
    * name
    * health
    * knockouts
    * hits
    * coconuts 
    * bananas
    * pinapples
    * points

client
* field_of_view (5x5 Matrix um ihn herum)
* items (dict mit item anzahl darin)
    * coconuts
    * bananas
    * pinapples
* round

## Matrix system

In der Matrix befinden sich nur Zahlen. Jede Zahl steht für ein Feld.

* 0 = Nichts ist in dem Feld
* <100 = Item oder Hinderniss Id
    * 1 = Dickicht
    * 2 = Kokosnuss
    * 3 = Banane
    * 4 = Ananass

* \>100 = player ID 
    * 101 = player 1
    * 102 = player 2
    * ...

Die Matrix ist wie ein Koordinatensystem aufgebaut, indem die Ecke links oben der Ursprung ist. Das Feld links oben in der Ecke auf unserem Spielfeld entspricht demnach dem Ursprung. 
Jedes Feld entspricht einer ganzzahligen Koordinate.  
Zugriff => Game.matrix[x][y]


