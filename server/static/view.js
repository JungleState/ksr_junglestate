// get endpoints and manage view

class View{
    constructor(field){
        this.field = field

        this.types = {
            "  ":"plain",
            "FF":"jungle",
            "CC":"coconut",
            "BB":"banana",
            "PP":"pineapple"
        }
    }

    getClassName(twoCharacterCode) {
        if (twoCharacterCode in this.types) {
            return this.types[twoCharacterCode];
        }
        return "player";
    }

    async updateSpectator() {
        let response = await fetch(`/view`);
        if (response.ok) {
            let json = await response.json();
            this.ShowStringField(json.field);
        }
    }

    ShowStringField(field) {
        const rows = this.field.getElementsByClassName('row');
        console.assert(rows.length == field.length, "Expected same number of rows");
        for (let r = 0; r <= rows.length; r++) {
            const viewRow = rows[r];
            const modelRow = field[r];
            const viewCells = viewRow.getElementsByTagName('cell');
            const cellCount = viewCells.length;
            console.assert(cellCount == modelRow.length / 2, "Expected same number of cells");
            for (let c = 0; c < cellCount; c++) {
                const cellContent = modelRow.slice(2*c, 2*c+2);
                const className = getClassName(cellContent)
                const viewCell = viewCells[c];
                // TODO: remove cell content
                viewCell.innerHTML = cellContent;
                viewCell.setAttribute("data-type", cellContent);
            }

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
