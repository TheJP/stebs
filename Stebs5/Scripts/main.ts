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
         * Hello callback function.
         */
        hello(word: string): void {
            ctx.fillStyle = 'green';
            ctx.font = '100pt Helvetica';
            ctx.fillText(word, canvas.width / 2, canvas.height / 2);
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

        setOutput(text: string): void {
            $('#outputText').text(text);
        }
    };

    export class Ram {
        private ramContent: number[];
        private lineBreak: number = 10;

        constructor(size: number, lineBreak: number) {
            this.lineBreak = lineBreak;

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

        public getAsString(): string {
            var asString: string = "";
            for (var i: number = 0; i < this.ramContent.length; i++) {
                if (i % this.lineBreak == 0) {
                    asString += "\n";
                }
                asString += this.ramContent[i].toString(16);
            }
            return asString;
        }
    };

    export var ramCont = new Stebs.Ram(16 * 16, 16);
}

/**
 * This interface allows the usage of the signalr library.
 */
interface JQueryStatic {
    connection: any;
}

$(document).ready(function (){

    $('#editorWindow').contents().prop('designMode', 'on');
    Stebs.ui.setupCanvas();

    var hub = $.connection.stebsHub;
    hub.client.hello = Stebs.ui.hello;

    $.connection.hub.start().done(function () {
        hub.server.hello('you');
    });

    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $('#openRam').click(Stebs.ui.toggleRAM);
    $('#openOutput').click(Stebs.ui.toggleOutput);

    $('#ramText').text(Stebs.ramCont.getAsString());
});