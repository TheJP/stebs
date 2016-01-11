module Stebs {
    export class Ram {
        private ramContent: number[];
        private ramToLine: number[];
        private isHighlighted: string[] = [];
        private isHidden: boolean = false;
        private stackPointerPos: number = 0;
        private instructionPointerPos: number = 0;

        constructor(size: number) {
            this.ramContent = Array(size);
            for (var i: number = 0; i < size; i++) {
                this.ramContent[i] = 0;
            }
        }

        /**
         * Initialice the ram.
         */
        public init() {
            var me = this;
            $('#ramTable').empty();
            $('#ramTable').append(this.getAsTable(16));
            $('#hideShowRam').click(function () {
                me.transactionHideShowRam();
                $('#ramTable').slideToggle();
            });
            $('#downloadRam').click(function () {
                console.log('implement download here');
            });
        }

        /**
         * Hide and show the ram
         */
        private transactionHideShowRam() {
            if (this.isHidden) {
                $('#hideShowRam').removeClass('arrowDownIcon')
                    .addClass('arrowUpIcon');
            } else {
                $('#hideShowRam').removeClass('arrowUpIcon')
                    .addClass('arrowDownIcon');
            }
            this.isHidden = !this.isHidden;
        }

        /**
         * Set the array containing with ram position points to witch line in the editor.
         * @param ram2Line The array containing the positions.
         */
        public setRamToLine(ramToLine: number[]): void {
            this.ramToLine = ramToLine;
        }

        /**
         * Get the linenumber of the instructionpointer
         * @param ipNr the instructionpointer
         */
        public getLineNr(ipNr: number): number {
            return this.ramToLine[ipNr];
        }

        /**
         * Set the ram content.
         * @param ram The array containing the new ram Data.
         */
        public setContent(ram: number[]): boolean {
            if (ram == null && ram.length != this.ramContent.length) {
                return false;
            } else {
                this.ramContent = ram;
                $('#ramTable').empty();
                $('#ramTable').append(this.getAsTable(16));
                return true;
            }
        }

        /**
         * Highlight the changed ram pos
         * @param elementName The elementname to highlight.
         */
        private highlight(elementName: string): void {
            this.resetHighlights();
            $(elementName).prop('class', 'changed');
            this.isHighlighted.push(elementName);
        }

        /**
         * Reset all highlighted lines.
         */
        public resetHighlights(): void {
            this.isHighlighted.forEach(element => {
                $(element).removeClass('changed');
            });
        }

        /**
         * Set the stackpointer to te given position.
         * @param position The new position of the stackpointer
         */
        public setStackPointer(position: number) {
            $('#cell-' + this.stackPointerPos).removeClass('stackPointer');
            $('#cell-' + position).addClass('stackPointer');
            this.stackPointerPos = position;
        }


        /**
         * Set the intructionpointer to the given position.
         * @param position The new position of the instructionpointer.
         */
        public setInstructionPointer(position: number) {
            $('#cell-' + this.instructionPointerPos).removeClass('instructionPointer');
            $('#cell-' + position).addClass('instructionPointer');
            this.instructionPointerPos = position;
        }

        /**
         * Change the ram at the given position to the new value.
         * @param pos The position to change the value;
         * @param value The new value witch will be set.
         */
        public setRamAt(pos: number, value: number): boolean {
            if (pos < 0 || pos >= this.ramContent.length || value < 0 || value > 255) {
                return false;
            }
            this.ramContent[pos] = value;
            $('#cell-' + pos).text(Stebs.utility.addLeadingZeros(value, 16, 2));
            this.highlight('#cell-' + pos);
            return true;
        }

        /**
         * Get the ramcontent as string (was used for testing).
         * @param lineBreak 
         */
        public getAsString(lineBreak: number): string {
            var asString: string = '';
            for (var i: number = 0; i < this.ramContent.length; i++) {
                if (i % lineBreak == 0) {
                    asString += '\n';
                }
                if (i % 2 == 0) {
                    asString += ' ';
                }
                asString += this.ramContent[i].toString(16);
            }
            return asString;
        }

        /**
         * Load the ram as Table.
         * @param lineLengh the length of a row
         */
        public getAsTable(lineLengh: number): HTMLTableElement {
            var table: HTMLTableElement;
            var thead: HTMLTableElement;
            var tbody: HTMLTableElement;

            table = document.createElement('table');
            //table.setAttribute('id', 'ramContent');
            thead = <HTMLTableElement> table.createTHead();
            tbody = <HTMLTableElement> table.createTBody();
            var hrow = <HTMLTableRowElement> table.tHead.insertRow(0);
            var bolt_elem_id = 'bold-elem';

            hrow.insertCell(0).innerHTML = '';
            for (var i: number = 0; i < lineLengh; i++) {
                var cell = hrow.insertCell(i + 1);
                cell.innerHTML = i.toString(16);
                cell.id = bolt_elem_id;
            }
            var newWith = (this.ramContent.length / (lineLengh));
            for (var i: number = 0; i < newWith; i++) {
                var row = <HTMLTableRowElement>tbody.insertRow();
                for (var j: number = 0; j < lineLengh; j++) {
                    if (j == 0) {
                        var cell = row.insertCell(0);
                        cell.id = bolt_elem_id;
                        cell.innerHTML = i.toString(16) + '0';
                    }
                    var cell = row.insertCell(j + 1);

                    cell.innerHTML = Stebs.utility.addLeadingZeros(this.ramContent[(i * newWith) + j], 16, 2);
                    cell.id = 'cell-' + ((i * newWith) + j);
                }
            }
            return table;
        }
    };

    /**
     * Create the ram class and set it to ramContent 
     * (Call ramContent if you want to interact with this class)
     */
    export var ramContent = new Stebs.Ram(256);
}
