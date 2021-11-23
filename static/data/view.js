// get endpoints and manage view

class View{
    constructor(field){
        this.field = field

        this.types = {
            0:"plain",
            1:"jungle",
            2:"coconut",
            3:"banana",
            4:"pineapple"
        }
    }
    Showfield(type, matrix){ //show the field //probably +playerdict (if we manage it in here)
        //The Matrix needs to be the same Dimensions as the given field of HTML Elements.   
        //Dimensions are set by the field automatically:

        let row=0
        for (let row_element of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            row+=1;
            let column=0;
            for(let tile of row_element.getElementsByTagName("tile")) { //Every Tile in each Row needs to be called "tile"
                column+=1;

                let tiletype = matrix[row][column];

                if(tiletype<100){ //Up to 100 are Items
                    tile.setAttribute('class', this.types[tiletype]); //adds the type from the Matrix to the tile
                }
                else{ //over 100 are the players
                    //tile.setAttribute('name', playerdict[tiletype-100]); ---- probably later or manage somewhere else
                    tile.setAttribute('name', "player");
                }
                
            }
        }

    }

}


