import "View_Spectator"

class Controller {
    constructor() {
        this.field == getData("field");

        setInterval(() => { // live updates
            View.ShowField(this.getData("field"));
        }, 500);
    }

    async getData(info) { // get certain info from app.py
        const response = await fetch("/view");
        const json = await response.json;

        switch(info) {
            case "field":
                return json.field;
            case "state":
                return json.state;
            case "round":
                return json.round;
            case "player_list":
                return json.player_list;
            default:
                return "error";
        }
    }

    async joinGame(playerName) {
        let mode = "spec";
        const response = await fetch("/view/${mode}/${playerName}");
        const json = await response.json;
    }

    // only for testing (probably)
    async randomPlayerName() {
        const response = await fetch("/uuid");
        const json = await response.json;
        return json.id;
    }
}

document.addEventListener("DOMContentLoaded", function() {
    controller = new Controller;
    view = new View;

    joinGame(randomPlayerName());
});