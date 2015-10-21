module Stebs {
    export var visible = {
        devices: false,
        architecture: false,
        ram: false,
        output: false
    };

    export var widths = {
        devices: '400px',
        architecture: '400px'
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
            if (visible.output) {
                $('#editorWindow').css({ height: visible.ram ? 'calc(100vh - 38px - 38px - 38px - 100px - 150px)' : 'calc(100vh - 38px - 38px - 38px - 100px - 300px)' });
            } else {
                $('#editorWindow').css({ height: visible.ram ? 'calc(100vh - 38px - 38px - 38px - 100px)' : 'calc(100vh - 38px - 38px - 38px - 100px - 150px)' });
            }
            $('.ram-container').hide(visible.ram);
            $('.ram-container').show(!visible.ram);
            $('.ram').css({ height: visible.ram ? '38px' : 'calc(38px + 150px)' });
            visible.ram = !visible.ram;
        },

        /**
         * Opens/Closes the output bar.
         */
        toggleOutput(): void {
            if (visible.ram) {
                $('#editorWindow').css({ height: visible.output ? 'calc(100vh - 38px - 38px - 38px - 100px - 150px)' : 'calc(100vh - 38px - 38px - 38px - 100px - 300px)' });
            } else {
                $('#editorWindow').css({ height: visible.output ? 'calc(100vh - 38px - 38px - 38px - 100px)' : 'calc(100vh - 38px - 38px - 38px - 100px - 150px)' });
            }
            $('.output-container').hide(visible.output);
            $('.output-container').show(!visible.output);
            $('.output').css({ height: visible.output ? '38px' : 'calc(38px + 150px)'});
            visible.output = !visible.output;
        }
    };
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

});