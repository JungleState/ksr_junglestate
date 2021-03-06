# Visualization

Die Visualisierungs-Komponente dient dazu, den ganzen Spielverlauf im Junglecamp für Zuschauer live darzustellen. Die verschiedenen Spieler sollten identifizierbar sein und deren Bewegungen und Aktionen zu erkennen sein.

## Mockup

![Mockup](monkeys.jpg)

## Technical Design

Die Visualisierung wird als Web-App implementiert und über JSON-Endpoints mit Daten beliefert.

![Komponentendiagramm](https://docs.google.com/drawings/d/e/2PACX-1vRAm6kpBC_tKuZkuTI6TLuOaZg-FCM23fvqHlSLsOAYu829yqgWOrzoolqrVyAQCf2EpRbXZKXuSvIg/pub?w=960&h=720)

Die App wird von einem dedizierten Pfad des Game-Servers (`/visualization/`) geladen und besteht aus drei Teilen:
1. Login-Seite (`/visualization/login.html`)
2. Anzeige der laufenden Spiele (`/visualization/games.html`)
3. Visualisierung eines einzelnen Spiels (`/visualization/game/<game_id>.html`)

Daten über die Spiele werden durch das folgende REST-API zur Verfügung gestellt:

Endpoint | Parameters | JSON Response | Example
-------- | ---------- | ------------- | -------
`/visualization/games.json` | *none* | `{ "games": [game URLs...] }` | <pre>{<br/>  "games": [     "/visualization/game/123",<br/>    "/visualization/game/42" <br/>  ]<br/> }</pre>
`/visualization/game/<game_id>.json` | *game_id*: Game-Id, wie von `/list` zurückgegeben. | <code>{ "id": <game_id>, "state": <game_state>, "cells": <array of strings> }</code> | <pre>{<br/>  "id": "123",<br/>  "state": "playing",<br/>  "cells": [ <br/>    "WWWWWWWWWWWWWWWWWWWWWWW",<br/>    "WW  PP               WW",<br/>    "WW             12    WW",<br/>    "WW    08  CC         WW",<br/>    "WWWWWWWWWWWWWWWWWWWWWWW"<br/>  ],<br/>  "players": {<br/>     "12": {<br/>      "id": "123",<br/>      "name": "Wally",<br/>      "lives": 3,<br/>      "ammo": 2<br/>     }, ...<br/>}</pre>

### Identifiers
Alle Identifier können beliebige Zeichenketten sein (also nicht nur Zahlen).

### Cell JSON
Jede Zelle des zweidimensionalen Spielfelds wird durch zwei Characters im String der jeweiligen Zeile codiert:

Name | Beispiel | Beschreibung
---- | -------- | ------------
leer | `"  "` | Leere Zelle
Wand | `"WW"` | Undurchdringliche Wand
Ananas | `"PP"` | Ananas Token
Banane | `"BB"` | Banane Token
Kokosnuss | `"CC"` | Kokosnuss Token
Spieler | `"08"` | Spieler-Id. Die Id ist nur eine Visualisierungs-Id und dient als Schlüssel im "players"-Objekt.

Die Informationen zu jedem Spieler unter der Visualisierungs-Id im `players`-Objekt unter folgenden Attributen abgelegt:

Name | Beispiel | Beschreibung
---- | -------- | ------------
`id` | `"123-ab"` | Eindeutiger Identifier des Spielers
`name` | `"Wally"` | Ein selbstgewählter Name des Spielers
`lives` | 4 | Verbleibende Lebensenergie des Spielers. Eine Energie von 0 bezeichnet ohnmächtige Spieler.
`ammo` | 3 | Verbleibende Kokosnüsse des Spielers.
`action`| `"move"` | Rundenaktion des Spielers. Mögliche Werte: <code>move, throw</code>
`direction` | `"left"` | Richtung der Rundenaktion. Mögliche Werte: <code>left, right, up, down</code>
`event` | `"powerup"` | Rundenereignis des Spielers. Mögliche Werte: <code>powerup, ammoup, hit, boink</code> (`boink` bezeichnet einen Zusammenstoss)

### Authentication

Alle Anfragen an das Visualisierungs-API müssen authentifiziert werden, um zu verhindern, dass die Spielfeldinformation von böswilligen Teilnehmern misbraucht werden kann. Die Index-Seite enthält dafür ein Passwort-Feld, in das das Visualisierungs-Passwort eingegeben werden muss. Im Server wird das gehashte Passwort mit einem hartcodierten (oder auf der Kommandozeile gegebenen) Hash verglichen und ein 403 zurückgegeben, wenn es ungültig ist.

### Long-polling etc.

Wie stellen wir sicher, dass der Visualisierungs-Client keine Updates verpasst? Über Client-Polling können wir nie ganz sicher sein, dass wir nicht eine Runde verpasst haben.

Ideen:
* Websockets: wir öffnen einen Websocket, der Server pusht alle updates über die Verbindung zum Client.
* HTTP 102 (Still Processing): Server gibt solange einen 102 (und den Inhalt) zurück, bis das Spiel beendet ist, worauf ein 200 erfolgt. Das tönt nach einem Hack...

### Nice to have

Folgende Dinge sind nicht zwingend, aber lohnenswerte Erweiterungen:
* Animation der Rundenaktionen & -ereignisse der Spieler
* Sounds bei bestimmten Ereignissen (Treffer, Tokens, Zusammenstoss)
* Skinning: wählbare Themes für das Spielfeld (Urwald, Pacman...)
