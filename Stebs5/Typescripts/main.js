/// <reference path="ram.ts"/>
var Stebs;
(function (Stebs) {
    Stebs.visible = {
        devices: false,
        architecture: false,
        ram: false,
        output: false
    };
    Stebs.widths = {
        devices: '250px',
        architecture: '400px'
    };
    Stebs.heights = {
        topbar: '38px',
        containerBar: '38px',
        containerSize: '150px',
        /* 1x topbar & 2x containerBar */
        bars: '114px',
        runAndDebug: '100px'
    };
    Stebs.ctrlStates = {
        start: function () {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon-grey.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon-grey.png");
            $('#btnPause').find('img').attr("src", "Icons/Pause-icon-grey.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon-grey.png");
            $('#btnDebug').prop('disabled', true);
            $('#btnRun').prop('disabled', true);
            $('#btnPause').prop('disabled', true);
            $('#btnStop').prop('disabled', true);
            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
        },
        assembled: function () {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPause').find('img').attr("src", "Icons/Pause-icon-grey.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon-grey.png");
            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPause').prop('disabled', true);
            $('#btnStop').prop('disabled', true);
            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
        },
        debuggingAndPause: function () {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPause').find('img').attr("src", "Icons/Pause-icon-grey.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon.png");
            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPause').prop('disabled', true);
            $('#btnStop').prop('disabled', false);
            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
        },
        instructionSteps: function () {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPause').find('img').attr("src", "Icons/Pause-icon.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon.png");
            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPause').prop('disabled', false);
            $('#btnStop').prop('disabled', false);
            $('.stepSizeRadioB').show();
            $('.stepSizeButtons').hide();
        },
        macroSteps: function () {
            Stebs.ctrlStates.instructionSteps();
        },
        microSteps: function () {
            Stebs.ctrlStates.instructionSteps();
        }
    };
    Stebs.ctrlState = Stebs.ctrlStates.start;
    Stebs.ctx;
    var canvas;
    Stebs.instructions;
    /**
     * The clientHub is a public singleton object, which contains client methods that can be called by the SignalR server.
     */
    Stebs.clientHub = {
        /**
         * Receive available assembly instructions from the server.
         * TODO: Add type to data.
         */
        instructions: function (data) {
            Stebs.instructions = data;
            //Simplify input for syntax highlighting
            for (var instruction in data) {
                assemblerInstruction[data[instruction].Mnemonic] = 'variable-2';
            }
        }
    };
    Stebs.ui = {
        /**
         * Stores a global reference of the canvas and sets the global style.
         */
        setupCanvas: function () {
            canvas = $('#canvas')[0];
            Stebs.ctx = canvas.getContext('2d');
            this.normalizeCanvas();
            Stebs.ctx.font = '20pt Helvetica';
            Stebs.ctx.textAlign = 'center';
        },
        /**
         * Resize canvas to real size (otherwise the content gets stretched).
         */
        normalizeCanvas: function () {
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
        setCodingViewWidth: function () {
            var width = (Stebs.visible.architecture ? ' - ' + Stebs.widths.architecture : '') + (Stebs.visible.devices ? ' - ' + Stebs.widths.devices : '');
            $('#codingView').css('width', 'calc(100% - 50px' + width + ')');
        },
        /**
         * Opens/Closes the devices sidebar.
         */
        toggleDevices: function () {
            var animation = { left: (Stebs.visible.devices ? '-=' : '+=') + Stebs.widths.devices };
            $('#devices, #architecture').animate(animation);
            var animation2 = { left: animation.left, width: (Stebs.visible.devices ? '+=' : '-=') + Stebs.widths.devices };
            $('#codingView').animate(animation2, Stebs.ui.setCodingViewWidth);
            Stebs.visible.devices = !Stebs.visible.devices;
        },
        /**
         * Opens/Closes the architecture sidebar.
         */
        toggleArchitecture: function () {
            var animation = { left: (Stebs.visible.architecture ? '-=' : '+=') + Stebs.widths.architecture };
            $('#architecture').animate(animation);
            var animation2 = { left: animation.left, width: (Stebs.visible.architecture ? '+=' : '-=') + Stebs.widths.architecture };
            $('#codingView').animate(animation2, Stebs.ui.setCodingViewWidth);
            Stebs.visible.architecture = !Stebs.visible.architecture;
        },
        /**
         * Sets the width of #codingFrame to a prozentual value.
         * This allows correct browser resizing without additional client code.
         */
        setCodingFrameHeight: function () {
            var height = (Stebs.visible.output ? ' - ' + Stebs.heights.containerSize : '') + (Stebs.visible.ram ? ' - ' + Stebs.heights.containerSize : '');
            $('#codingFrame').css('height', 'calc(100% - ' + Stebs.heights.bars + ' - ' + Stebs.heights.runAndDebug + height + ')');
        },
        /**
         * Opens/Closes the ram bar.
         */
        toggleRAM: function () {
            $('#codingFrame').animate({ height: (Stebs.visible.ram ? '+=' : '-=') + Stebs.heights.containerSize }, Stebs.ui.setCodingFrameHeight);
            Stebs.visible.ram = !Stebs.visible.ram;
            if (Stebs.visible.ram) {
                $('.ram-container').slideDown();
            }
            else {
                $('.ram-container').slideUp();
            }
        },
        /**
         * Opens/Closes the output bar.
         */
        toggleOutput: function () {
            $('#codingFrame').animate({ height: (Stebs.visible.output ? '+=' : '-=') + Stebs.heights.containerSize }, Stebs.ui.setCodingFrameHeight);
            Stebs.visible.output = !Stebs.visible.output;
            if (Stebs.visible.output) {
                $('.output-container').slideDown();
            }
            else {
                $('.output-container').slideUp();
            }
        },
        openOutput: function () {
            if (!Stebs.visible.output) {
                this.toggleOutput();
            }
        },
        setOutput: function (text) {
            $('#outputText').text(text);
        }
    };
    Stebs.ramCont = new Stebs.Ram(1024);
})(Stebs || (Stebs = {}));
$(document).ready(function () {
    Stebs.ui.setupCanvas();
    var hub = $.connection.stebsHub;
    hub.client.instructions = Stebs.clientHub.instructions;
    $.connection.hub.start().done(function () {
        //Get available assembly instructions
        hub.server.getInstructions();
    });
    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $('#openRam').click(Stebs.ui.toggleRAM);
    $('#openOutput').click(Stebs.ui.toggleOutput);
    $('.ram-container').append(Stebs.ramCont.getAsTable(16 * 4));
    hub.client.assembled = function (result) {
        Stebs.ui.openOutput();
        var output = $('#outputText');
        output.text(result);
        output.html(output.html().replace(/\n/g, '<br/>').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;').replace(/\s/g, '&nbsp;'));
    };
    hub.client.assembleError = hub.client.assembled;
    $('#btnAssemble').click(function () {
        var newSource = editor.getDoc().getValue().replace(/\r?\n/g, '\r\n');
        $.connection.stebsHub.server.assemble(newSource);
        Stebs.ctrlStates.assembled();
    });
    $('#btnRun').click(function () {
        Stebs.ctrlStates.instructionSteps();
    });
    $('#btnDebug').click(function () {
        Stebs.ctrlStates.debuggingAndPause();
    });
    var editor = CodeMirror.fromTextArea($('#editableTxtArea').get(0), {
        lineNumbers: true,
        mode: 'assembler'
    });
    Stebs.ctrlStates.start();
});
