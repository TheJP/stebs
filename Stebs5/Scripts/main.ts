/// <reference path="editorWindow.ts"/>

module Stebs {
    export var visible = {
        devices: false,
        architecture: false,
        ram: false,
        output: false
    };

    export var widths = {
        devices: '250px',
        architecture: '400px'
    };

    export var heights = {
        topbar: '38px',
        containerBar: '38px',
        containerSize: '150px',
        /* 1x topbar & 2x containerBar */
        bars: '114px'
    };

    var ctx: CanvasRenderingContext2D;
    var canvas: HTMLCanvasElement;

    export var ui = {

        /**
         * Stores a global reference of the canvas and sets the global style.
         */
        setupCanvas(): void {
            canvas = <HTMLCanvasElement>$('#canvas')[0];
            ctx = canvas.getContext('2d');
            this.normalizeCanvas();

            ctx.font = '20pt Helvetica';
            ctx.textAlign = 'center';
        },

        /**
         * Resize canvas to real size (otherwise the content gets stretched).
         */
        normalizeCanvas(): void {
            var width = parseInt($('#canvas').css('width'), 10);
            var height = parseInt($('#canvas').css('height'), 10);
            if (canvas.width != width || canvas.height != height) {
                canvas.width = width;
                canvas.height = height;
            }
        },

        /**
         * Sets the width of #codingView to a prozentual value.
         * This allows correct browser resizing without additional client code.
         */
        setCodingViewWidth(): void {
            var width = (visible.architecture ? ' - ' + widths.architecture : '') + (visible.devices ? ' - ' + widths.devices : '');
            $('#codingView').css('width', 'calc(100% - 50px' + width + ')');
        },

        /**
         * Opens/Closes the devices sidebar.
         */
        toggleDevices(): void {
            var animation = { left: (visible.devices ? '-=' : '+=') + widths.devices };
            $('#devices, #architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.devices ? '+=' : '-=') + widths.devices };
            $('#codingView').animate(animation2, ui.setCodingViewWidth);
            visible.devices = !visible.devices;
        },

        /**
         * Opens/Closes the architecture sidebar.
         */
        toggleArchitecture(): void {
            var animation = { left: (visible.architecture ? '-=' : '+=') + widths.architecture };
            $('#architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.architecture ? '+=' : '-=') + widths.architecture };
            $('#codingView').animate(animation2, ui.setCodingViewWidth);
            visible.architecture = !visible.architecture;
        },

        /**
         * Opens/Closes the ram bar.
         */
        toggleRAM(): void {
            $('#codingFrame').animate({ height: (visible.ram ? '+=' : '-=') + heights.containerSize }, function () {
                if (visible.output) {
                    $('#codingFrame').css({ height: 'calc(100% - ' + heights.bars + ' - ' + heights.containerSize + (visible.ram ? '' : ' - ' + heights.containerSize) +')' });
                } else {
                    $('#codingFrame').css({ height: 'calc(100% - ' + heights.bars + (visible.ram ? '' : ' -' + heights.containerSize) + ')' });
                }
                visible.ram = !visible.ram;
            });
            $('.ram').animate({ height: heights.containerBar + (visible.ram ? '' : ' + ' + heights.containerSize) +  ')' });
            $('.ram-container').hide(visible.ram);
            $('.ram-container').show(!visible.ram);
        },

        /**
         * Opens/Closes the output bar.
         */
        toggleOutput(): void {
            $('#codingFrame').animate({ height: (visible.output ? '+=' : '-=') + heights.containerSize }, function () {
                if (visible.ram) {
                    $('#codingFrame').css({ height: 'calc(100% - ' + heights.bars + ' - ' + heights.containerSize + (visible.output ? '' : ' - ' + heights.containerSize) + ')' });
                } else {
                    $('#codingFrame').css({ height: 'calc(100% - ' + heights.bars + (visible.output ? '' : ' -' + heights.containerSize) + ')' });
                }
                visible.output = !visible.output;
            });
            $('.output').animate({ height: heights.containerBar + (visible.output ? '' : ' + ' + heights.containerSize) + ')' });
            $('.output-container').hide(visible.output);
            $('.output-container').show(!visible.output);
        },
        openOutput(): void {
            if (!visible.output) { this.toggleOutput(); }
        },

        setOutput(text: string): void {
            $('#outputText').text(text);
        }
    };

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
                    cell.innerHTML = this.ramContent[(i * newWith) + (j * 2)].toString(16) + this.ramContent[(i * newWith) + (j*2) + 1].toString(16);
                }
            }
            return table;
        }
    };

    export var ramCont = new Stebs.Ram(1024);

    /**
     * This interface allows the usage of the CodeMirror library.
     */
    export interface CodeMirror {
        codeMirror: any;
    }
}

/**
 * This interface allows the usage of the signalr library.
 */
interface JQueryStatic {
    connection: any;
}

interface JQuery {
    
}




$(document).ready(function (){
    Stebs.ui.setupCanvas();

    var hub = $.connection.stebsHub;
    //hub.client.hello = Stebs.ui.hello;

    $.connection.hub.start().done(function () {
        //hub.server.hello('you');
    });

    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $('#openRam').click(Stebs.ui.toggleRAM);
    $('#openOutput').click(Stebs.ui.toggleOutput);

    $('.ram-container').append(Stebs.ramCont.getAsTable(16 * 4));

    hub.client.assembled = function (result: string) {
        Stebs.ui.openOutput();
        var output = $('#outputText');
        output.text(result);
        output.html(output.html().replace(/\n/g, '<br/>').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;').replace(/\s/g, '&nbsp;'));
    };
    hub.client.assembleError = hub.client.assembled;

    $('#assemble').click(function () {
        var source = $('#editorWindow').contents().find('body').html().replace(/<\w*br\w*\/?>/g, '\r\n').replace(/<.*>/g, '');
        $.connection.stebsHub.server.assemble(source);
    });

    /*
    var editorWindow: Stebs.EditorWindow = new Stebs.EditorWindow();
    window.setInterval(function () {
        editorWindow.inkText();
    }, 20000);
    */
    var editor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#editableTxtArea').get(0), {
        lineNumbers: true,
        mode: { name: 'gas', architecture: 'x86' }
    });

});
