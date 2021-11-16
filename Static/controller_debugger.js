class Controller {
    constructor() {
        this.field == this.getData("field");

        // listen for input
        window.onkeydown = (key) => {
            this.keyInput(key.keyCode);
        }

        // field updates
        setInterval(() => {
            View.Showfield("spectator", this.getData("field"));
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

    async keyInput(commandKey) { // send command to app.py
        // # move_id list:
        // # 0: Stay
        // # 1: Move
        // # 2: Shoot
        // #
        // # dir list:
        // # -1: No direction
        // # 0: up
        // # 1: up right
        // # 2: right
        // # 3: down right
        // # 4: down
        // # 5: down left
        // # 6: left
        // # 7: up left

        // 8:"backspace", 9:"tab", 13:"return", 16:"shift", 17:"ctrl", 18:"alt", 19:"pausebreak", 20:"capslock", 27:"escape", 32:" ", 33:"pageup",
        // 34:"pagedown", 35:"end", 36:"home", 37:"left", 38:"up", 39:"right", 40:"down", 43:"+", 44:"printscreen", 45:"insert", 46:"delete",
        // 48:"0", 49:"1", 50:"2", 51:"3", 52:"4", 53:"5", 54:"6", 55:"7", 56:"8", 57:"9", 59:";",
        // 61:"=", 65:"a", 66:"b", 67:"c", 68:"d", 69:"e", 70:"f", 71:"g", 72:"h", 73:"i", 74:"j", 75:"k", 76:"l",
        // 77:"m", 78:"n", 79:"o", 80:"p", 81:"q", 82:"r", 83:"s", 84:"t", 85:"u", 86:"v", 87:"w", 88:"x", 89:"y", 90:"z",
        // 96:"0", 97:"1", 98:"2", 99:"3", 100:"4", 101:"5", 102:"6", 103:"7", 104:"8", 105:"9",
        // 106: "*", 107:"+", 109:"-", 110:".", 111: "/",
        // 112:"f1", 113:"f2", 114:"f3", 115:"f4", 116:"f5", 117:"f6", 118:"f7", 119:"f8", 120:"f9", 121:"f10", 122:"f11", 123:"f12",
        // 144:"numlock", 145:"scrolllock", 186:";", 187:"=", 188:",", 189:"-", 190:".", 191:"/", 192:"`", 219:"[", 220:"\\", 221:"]", 222:"'"
    
        let type = "move";
        let direction = "none";
        switch(commandKey) {
            case 87: // w
                type = 1;
                direction = 0;
                break;
            case 65: // a
                type = 1;
                direction = 6;
                break;
            case 83: // s
                type = 1;
                direction = 4;
                break;
            case 68: // d
                type = 1;
                direction = 2;
                break;
            case 85: // u
                type = 2;
                direction = 7;
                break;
            case 73: // i
                type = 2;
                direction = 0;
                break;
            case 79: // o
                type = 2;
                direction = 1;
                break;
            case 74: // j
                type = 2;
                direction = 6;
                break;
            case 97: // l
                type = 2;
                direction = 2;
                break;
            case 78: // n
                type = 2;
                direction = 5;
                break;
            case 77: // m
                type = 2;
                direction = 4;
                break;
            case 188: // ,
                type = 2;
                direction = 3;
                break;
            default:
                type = 0;
                direction = -1;
        }

        const response = await fetch("/action/${type}/${direction}");
        const json = await response.json;
    }

}

document.addEventListener("DOMContentLoaded", function() {
    view = new View;
    controller = new Controller;
});