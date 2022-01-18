// get endpoints and manage view

class View{
    constructor(field){
        this.field = field
    }
    Showfield(type, matrix){ //show the field
        if (type=='spectator'){ //Dimensions for spectator
            let width= 50; 
            let height=50;
        }
        if (type=='client'){ //Dimensions for human player
            let widht= 5;
            let height=5;
        }
        let row=0
        for (let row_element of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            row+=1
            let column=0
            for(let tile of row_element.getElementsByTagName("tile")) { //Every Tile in each Row needs to be called "tile"
                column+=1
                let tiletype = matrix[row][column]

                if(tiletype==0){ //probably solve this with dicts (MORE BEAUTIFUL)
                    tile.classList.add('plain')
                }
                if(tiletype==1){
                    tile.classList.add('jungle')
                }


            }
        }

    }

}