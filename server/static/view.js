// get endpoints and manage view

class View{
    constructor(grid, navigation){
        this.field = grid;
        this.navigation = navigation;

        this.types = {
            "  ":"plain",
            "FF":"jungle",
            "CC":"coconut",
            "BB":"banana",
            "PP":"pineapple"
        }
    }
    updateView(json){ //show the field
        //The Matrix should be the same Dimensions as the given field of HTML Elements.   
        //Dimensions are set by the field automatically:
        let stringfield = json.field
        let row=0;
        for (let row_element of this.field.getElementsByClassName("row")) { //come from HTML
            let column=0;

            for(let tile of row_element.getElementsByTagName("cell")) { 

                let charcode = stringfield[row].slice(column, column+2); //this cuts the 2 letters out of the string. the end(column+2) is exclusive
                tile.innerHTML="";

                if(isNaN(parseInt(charcode, 10))){ //parseInt returns NaN (Not a Number) if there is no number
                    tile.setAttribute('class', this.types[charcode]); //adds the type from the Matrix to the tile
                }
                else{ //numbers are players
                    //tile.setAttribute('name', playerdict[charcode]);
                    tile.setAttribute('class', "player");
                    tile.setAttribute('id', Object.keys(json.name_list)[parseInt(charcode)]); //I need to append uuid for coconut throwing
                    var playername = document.createElement("playername");
                    tile.appendChild(playername);
                    playername.textContent= Object.values(json.name_list)[parseInt(charcode)];
                }
                column+=2; //because every tile consists of 2 letters.
                
            }
            row+=1;
        }
        //for the shoot animation if players shoot coconut
        for(let player in json.projectiles){
            this.shoot(player, json.projectiles.direction) //fix, not right yet: do tis<-------------------------------------------------------------------------------------------------------------------------
        }


        // Display stats
        if (json.mode == 'spec') {
            this.specMode(json);
        }
        else if (json.mode == 'client') {
            this.clientMode(json);
        }
    }

    shoot(uuid, direction){ //creates an projectile element with direction property where the player is.

        var player = document.getElementById(uuid);
        var projectile = document.createElement("projectile", {direction: direction});
        player.appendChild(projectile);
        
    }

    specMode(json) {
        let l = json.scoreboard.length;
        let navigation = document.getElementById('navigation');

        navigation.innerHTML = '';

        let title = document.createElement('div');
        title.classList.add('title');
        title.innerText = 'Scoreboard - (Points/Lives/Nuts)';
        navigation.appendChild(title);

        for (let i = 0; i < l; i++) {
            let div = document.createElement('div');
            div.classList.add('playerSb');
            let player = json.scoreboard[i];
            if (!player.active) {
                console.log("inactive");
                div.classList.add('inactive');
            }
            div.innerText = `${i+1}. ${player.name} (${player.points}/${player.lives}/${player.coconuts}): ${player.message}`;
            navigation.appendChild(div);
        }
    }

    clientMode(json) {
        this.navigation.querySelector('#name').innerHTML = `Logged in as: ${json.name}`;
        this.navigation.querySelector('#coconuts').innerHTML = `Coconuts: ${json.coconuts}`;
        this.navigation.querySelector('#lives').innerHTML = `Lives: ${json.lives}`;
        this.navigation.querySelector('#points').innerHTML = `Points: ${json.points}`;
        this.navigation.querySelector('#round').innerHTML = `Round: ${json.round}`;
    }
}

function test(){
    var testfield =    ["FFFFFFFFFFFFFFFF",
                        "FF01BB99",
                        "  BB  BBBBBB", 
                        "      CC", 
                        "BB    "]

    const view = new View(document.getElementById("grid"));
    view.Showfield(testfield);
}
