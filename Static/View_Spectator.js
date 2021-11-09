// get endpoints and manage view

class View{
    constructor(field){
        this.field = field

        this.types = {
            0: "plain",
            1: "jungle",
            2: "coconut",
            3: "banana",
            4: "pineapple"
        }

    }
    Showfield(matrix){ //show the field

        width= 50; //Dimensions
        height=50;

        row=0
        for (let rowe_lement of this.field.getElementsByClassName("row")) { //needs Rows in HTML (so I get Rows)
            row+=1
            column=0
            for(let tile of row_element.getElementsByTagName("tile")) { //Every Tile in each Row needs to be called "tile"
                column+=1
                tiletype = matrix[row][column]

                tile.classList.add(this.types[tiletype]) //adds the 


            }
        }

    }

}
