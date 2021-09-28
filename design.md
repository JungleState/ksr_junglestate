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

## Implementation of REST-endpoints

Website (Rückgabe in HTML)
* /: Gibt view Website

Spielsuche (Rückgabe in JSON):
* /joinGame: POST; Tritt Spiel bei, wenn keins vorhanden: erstellt eins

Aktives Spiel (Rückgabe in JSON):
* /player/client/\<gameID>/\<playerID>/\<Action>: POST; Aktion an Server, Spielstatus zurück
* /spectator/view/\<gameID>: GET; Gibt Spielmatrix zurück

## API


