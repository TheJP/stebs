module Stebs {
    export class Ram {
        private ramContent: number[];
        private ram2Line: number[];
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

        public init() {
            var me = this;
            $('#ramDevice').append(this.getAsTable(16));
            $('#hideShowRam').click(function () {
                me.transactionHideShowRam();
            });
            $('#downloadRam').click(function () {
                console.log('implement download here');
            });
        }

        private transactionHideShowRam() {
            if (this.isHidden) {
                $('#ramDevice').animate({ height: '380px' });
                $('#hideShowRam').removeClass('arrowDownIcon')
                    .addClass('arrowUpIcon');
            } else {
                $('#ramDevice').animate({ height: '25px' });
                $('#hideShowRam').removeClass('arrowUpIcon')
                    .addClass('arrowDownIcon');
            }
            this.isHidden = !this.isHidden;
        }

        public setRam2Line(ram2Line: number[]): void {
            this.ram2Line = ram2Line;
        }

        public setContent(ram: number[]): boolean {
            if (ram == null && ram.length != this.ramContent.length) {
                return false;
            } else {
                this.ramContent = ram;
                this.init();
                return true;
            }
        }

        private highlight(elementName: string): void {
            this.resetHighlights();
            $(elementName).prop('class', 'changed');
            this.isHighlighted.push(elementName);
        }

        private resetHighlights(): void {
            this.isHighlighted.forEach(element => {
                $(element).removeClass('changed');
            });
        }

        public setStackPointer(position: number) {
            $('#cell-' + this.stackPointerPos).removeClass('stackPointer');
            $('#cell-' + position).addClass('stackPointer');
            this.stackPointerPos = position;
        }

        public setInstructionPointer(position: number) {
            $('#cell-' + this.instructionPointerPos).removeClass('instructionPointer');
            $('#cell-' + position).addClass('instructionPointer');
            this.instructionPointerPos = position;
        }

        public setRamAt(pos: number, value: number): boolean {
            if (pos < 0 || pos >= this.ramContent.length || value < 0 || value > 255) {
                return false;
            }
            this.ramContent[pos] = value;
            $('#cell-' + pos).text(Stebs.utility.addLeadingZeros(value, 16, 2));
            this.highlight('#cell-' + pos);
            return true;
        }

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
}
