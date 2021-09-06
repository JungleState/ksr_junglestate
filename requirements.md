# Requirements

JungleState möchte einen Game-Server entwickeln lassen, der remote Clients in einem rundenbasierten Game gegeneinander spielen lässt. Die _Requirements_ definieren die Vorgaben zum Spiel und zur gewählten Technologie.

## Spielbeschrieb

Das Spiel besteht aus dem Urwald und Affen, die sich gegenseitig mit Kokosnüssen bewerfen.

### Spielfeld
Der Urwald besteht aus einer zweidimensionalen Matrix mit diskreten Feldern. Jedes Feld kann durch einen eindeutigen Zustand beschrieben werden, zum Beispiel
  * frei
  * Dickicht (undurchdringlich)
  * Affe
  * Tokens wie Banane, Kokosnuss, ev. weitere Tokens

### Spielablauf
Jeder Affe erhält zu Beginn der Runde seinen einsehbaren Teilbereich des Urwalds. Dieser beträgt typischerweise 5x5 Felder um die Position des Affen. Der Affe entscheidet sich für eine Aktion, entweder `MOVE` oder `THROW_COCONUT` in einer der 8 Richtungen. Kokosnüsse fliegen nur ein Feld weit.

Wird ein Affe von einer Kokosnuss getroffen, wird seine Lebensenergie um eine bestimmte Menge verringert. Fällt die Lebensenergie eines Affen auf null, fällt er ohnmächtig vom Ast und scheidet aus<sup>[1](#foot1)</sup>.

Kollidieren zwei Affen oder ein Affe und der Dickicht, verringert sich die Lebensenergie ebenfalls, die Affen bleiben auf ihrem Feld stehen.

Findet ein Affe eine Banane, wird die Lebensenergie erhöht. Gefundene Kokosnüsse erhöhen den Munitionsvorrat. Sowohl Lebensenergie als auch Kokosnussvorrat sind begrenzt.

<a name="#foot1">[1]</a>: Selbstverständlich erwachen alle ohnmächtige Affen nach dem Spielende ohne bleibende Schäden zu erleiden. Bei der Entwicklung dieses Spiels dürfen keine Tiere zu Schaden kommen!

### Punkte, Spielende, Gewinn
Affen erhalten Punkte für gewisse Aktionen:
  * Treffen von Gegnern
  * Eliminieren von Gegnern
  * ev. Finden von bestimmten Tokens (Ananas?)

Das Spiel endet wenn weniger als zwei Affen übrig bleiben, oder nach einer definierten Zeit.

Gewinner ist der Affe mit den meisten Punkten. Eventuell könnte man den Gewinner auch nur aus den wachen Affen auswählen - allerdings schafft dies einen Anreiz, sich möglichst lange zu verstecken und am Schluss auf den letzten Überlebenden loszugehen...

## Technische Vorgaben

Die Umsetzung folgt der [Client-Server](https://de.wikipedia.org/wiki/Client-Server-Modell)-Architektur.

### Server
Der Server wird als [REST](https://de.wikipedia.org/wiki/Representational_State_Transfer)-API definiert und implementiert. Das API soll mit der Anfangsanfrage mitgeliefert werden ([HATEOS](https://de.wikipedia.org/wiki/Representational_State_Transfer#Beispiel)).
  * Die Abfrage des Spielzustands soll mittels Long-Polling (ev. [Websockets](https://en.wikipedia.org/wiki/WebSocket) in [Quart](https://gitlab.com/pgjones/quart)) umgesetzt werden: der Client fragt den Zustand an, die Antwort erfolgt erst, wenn die Runde zuende ist.
  * Der Server unterstützt zusätzlich zum Client-Zugang über einen privilegierten Zugang, um das gesamte Spielfeld zu visualisieren.
  * Es werden verschiedene Spielfelder unterstützt (default, Testumgebung, Schwierigkeitsgrade...).
  * Ein Spiel soll von bis zu 50 Clients gleichzeitig gespielt werden können.
  * Clients, die zulange mit der Antwort zuwarten, werden aus dem Spiel ausgeschlossen.

### Client
Clients greifen ausschliesslich über das REST-API auf ein Spiel zu. Zum Lieferumfang gehören:
  * Ein Beispiel-Client auf C#-Basis, der einen einfachen Affen-Agenten implementiert (z.B. einer, der jede zweite Runde in einer zufälligen Richtung geht und auf alles in der Umgebung schiesst). 
  * Ein Konsolen- oder Web-Client-Programm für Testzwecke, das die manuelle Steuerung eines Affen erlaubt.
    * Die Steuerung soll über Tastatureingabe erfolgen.
  * Eine Visualisierung als Web-App, die das gesamte Spielfeld abbildet, um Spiele live verfolgen zu können.
    * Die App soll den Namen _Jungle Camp_ tragen.
