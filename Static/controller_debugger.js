import "View_Debugger"

class Controller {
    constructor() {
        this.field == getData("field");

        // listen for input
        window.onkeydown = function(key) {
            this.input(key.keycode);
        }

        // field updates
        setInterval(() => {
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
        }
    }

    async input(commandKey) { // send command to app.py
        //012
        //345
        //678

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
                type = "move";
                direction = 1;
            case 65: // a
                type = "move";
                direction = 3;
            case 83: // s
                type = "move";
                direction = 7;
            case 68: // d
                type = "move";
                direction = 3;
            case 85: // u
                type = "attack";
                direction = 0;
            case 73: // i
                type = "attack";
                direction = 1;
            case 79: // o
                type = "attack";
                direction = 2;
            case 74: // j
                type = "attack";
                direction = 3;
            case 97: // l
                type = "attack";
                direction = 5;
            case 78: // n
                type = "attack";
                direction = 6;
            case 77: // m
                type = "attack";
                direction = 7;
            case 188: // ,
                type = "attack";
                direction = 8;
            default:
                type = "move";
                direction = "none";
        }
        alert(type+direction);
        // const response = await fetch("/action/${type}/${direction}");
        // const json = await response.json;
    }

}

document.addEventListener("DOMContentLoaded", function() {
    controller = new Controller;
    view = new View;
});