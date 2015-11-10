module Stebs {
    export class Ram {
        private ramContent: number[];

        constructor(size: number) {
            this.ramContent = Array(size);
            for (var i: number = 0; i < size; i++) {
                this.ramContent[i] = 0;
            }
        }

        public setContent(exising: string): boolean {
            if (exising == null) {
                return false;
            }

            return true;
        }

        public setRamAt(pos: number, val: number): boolean {
            if (pos < 0 || pos >= this.ramContent.length || val < 0 || val > 255) {
                return false;
            }
            this.ramContent[pos] = val;
            return true;
        }

        public getAsString(lineBreak: number): string {
            var asString: string = "";
            for (var i: number = 0; i < this.ramContent.length; i++) {
                if (i % lineBreak == 0) {
                    asString += "\n";
                }
                if (i % 2 == 0) {
                    asString += " ";
                }
                asString += this.ramContent[i].toString(16);
            }
            return asString;
        }

        public getAsTable(lineLengh: number): HTMLTableElement {
            var table: HTMLTableElement;
            var thead: HTMLTableElement;
            var tbody: HTMLTableElement;

            table = document.createElement('table');
            //table.setAttribute("id", "ramTable");
            thead = <HTMLTableElement> table.createTHead();
            tbody = <HTMLTableElement> table.createTBody();
            var hrow = <HTMLTableRowElement> table.tHead.insertRow(0);

            hrow.insertCell(0).innerHTML = "";
            for (var i: number = 0; i < lineLengh / 2; i++) {
                var cell = hrow.insertCell(i + 1);
                cell.innerHTML = i.toString(16);
            }
            var newWith = (this.ramContent.length / (lineLengh));
            for (var i: number = 0; i < newWith / 2; i++) {
                var row = <HTMLTableRowElement> table.tHead.insertRow(i + 1);
                for (var j: number = 0; j < lineLengh / 2; j++) {
                    if (j == 0) {
                        var cell = row.insertCell(0);
                        cell.innerHTML = i.toString(16);
                    }
                    var cell = row.insertCell(j + 1);
                    cell.innerHTML = this.ramContent[(i * newWith) + (j * 2)].toString(16) + this.ramContent[(i * newWith) + (j * 2) + 1].toString(16);
                }
            }
            return table;
        }
    };
}
