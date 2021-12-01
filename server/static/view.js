// get endpoints and manage view

class View{
    constructor(){
        this.field = document.getElementById("grid");

        this.types = {
            "  ":"plain",
            "FF":"jungle",
            "CC":"coconut",
            "BB":"banana",
            "PP":"pineapple"
        }
    }
    Showfield(stringfield){ //show the field //probably +playerdict (if we manage it in here)
        //The Matrix should be the same Dimensions as the given field of HTML Elements.   
        //Dimensions are set by the field automatically:

        let row=0
        for (let row_element of this.field.getElementsByClassName("row")) { //come from HTML
            let column=0;

            for(let tile of row_element.getElementsByTagName("cell")) { 

                let charcode = stringfield[row].slice(column, column+2); //this cuts the 2 letters out of the string. the end(column+2) is exclusive

                if(isNaN(parseInt(charcode, 10))){ //parseInt returns NaN (Not a Number) if there is no number
                    tile.setAttribute('class', this.types[charcode]); //adds the type from the Matrix to the tile
                }
                else{ //numbers are players
                    //tile.setAttribute('name', playerdict[charcode]); ---- probably later or manage somewhere else
                    tile.setAttribute('name', "player");
                }
                column+=2; //because every 
                
            }
            row+=1;
        }

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
