// get endpoints and manage view

class View{
    constructor(field){
        this.field = field
    }
    Showfield(type, matrix){ //show the field
        if (type=='spectator'){ //Dimensions for spectator
            width= 50; 
            height=50;
        }
        if (type=='player'){ //Dimensions for human player
            widht= 5;
            height=5;
        }
        row=0
        for (let rowe_lement of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            row+=1
            column=0
            for(let tile of row_element.getElementsByTagName("tile")) { //Every Tile in each Row needs to be called "tile"
                column+=1
                tiletype = matrix[row][column]

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

class Connector{

}