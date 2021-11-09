// get endpoints and manage view

class View{
    constructor(field){
        this.field = field

        this.types = [
            "plain",
            "jungle",
            "coconut",
            "banana",
            "pineapple"
        ]

    }
    Showfield(matrix, playerdict){ //show the field
        //The Matrix needs to be the same Dimensions as the given field of HTML Elements.   

        let row=0
        for (let row_element of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            row+=1;
            let column=0;
            for(let tile of row_element.getElementsByTagName("tile")) { //Every Tile in each Row needs to be called "tile"
                column+=1;

                tiletype = matrix[row][column];
                if(tiletype<100){ //Up to 100 are Items
                    tile.setAttribute('class', this.types[tiletype]); //adds the type from the Matrix to the tile
                }
                else{ //over 100 are the players
                    tile.setAttribute('name', playerdict[tiletype-100]); //shouldn't be a dict but a list
                }
                
            }
        }

    }

}
