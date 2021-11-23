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
    Showfield(matrix){ //show the field //probably +playerdict (if we manage it in here)
        //The Matrix needs to be the same Dimensions as the given field of HTML Elements.   
        //Dimensions are set by the field automatically:

        let row=0
        for (let row_element of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            let column=0;

            for(let tile of row_element.getElementsByTagName("cell")) { //Every Tile in each Row needs to be called "tile"

                let tiletype = matrix[row][column];

                if(tiletype<100){ //Up to 100 are Items
                    tile.setAttribute('class', this.types[tiletype]); //adds the type from the Matrix to the tile
                }
                else{ //over 100 are the players
                    //tile.setAttribute('name', playerdict[tiletype-100]); ---- probably later or manage somewhere else
                    tile.setAttribute('name', "player");
                }
                column+=1;
                
            }
            row+=1;
        }

    }

}

function test(){
    var testmatrix =   [[1, 0, 0, 0, 1],
                        [1, 1, 0, 1, 1],
                        [1, 0, 1, 0, 1], 
                        [1, 0, 1, 1, 1], 
                        [1, 1, 1, 1, 0]]

    const view = new View(document.getElementById("grid"));
    view.Showfield(testmatrix);

}
