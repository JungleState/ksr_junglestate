import "View_Spectator"

class Controller {
    constructor() {
        this.field == getData("field");
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
        }
    }

    setInterval(() => {
        View.ShowField(getData("field"));
    }, 500);
}