﻿/// <reference path="ram.ts"/>

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
        bars: '114px',
        runAndDebug: '100px'
    };

    export var ctrlStates = {
        start(): void {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon-grey.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon-grey.png");
            $('#btnPauseRun').find('img').attr("src", "Icons/Pause-icon-grey.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon-grey.png");
            
            $('#btnDebug').prop('disabled', true);
            $('#btnRun').prop('disabled', true);
            $('#btnPauseRun').prop('disabled', true);
            $('#btnStop').prop('disabled', true);

            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
            Stebs.ctrlState = Stebs.ctrlStates.start;
        },
        assembled(): void {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPauseRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon-grey.png");

            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPauseRun').prop('disabled', false);
            $('#btnStop').prop('disabled', true);

            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
            Stebs.ctrlState = Stebs.ctrlStates.assembled;
        },
        debuggingAndPause(): void {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPauseRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon.png");

            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPauseRun').prop('disabled', false);
            $('#btnStop').prop('disabled', false);

            $('.stepSizeRadioB').hide();
            $('.stepSizeButtons').show();
            Stebs.ctrlState = Stebs.ctrlStates.debuggingAndPause;
        },
        instructionSteps(): void {
            $('#btnDebug').find('img').attr("src", "Icons/Debug-icon.png");
            $('#btnRun').find('img').attr("src", "Icons/Play-icon.png");
            $('#btnPauseRun').find('img').attr("src", "Icons/Pause-icon.png");
            $('#btnStop').find('img').attr("src", "Icons/Stop-icon.png");

            $('#btnDebug').prop('disabled', false);
            $('#btnRun').prop('disabled', false);
            $('#btnPauseRun').prop('disabled', false);
            $('#btnStop').prop('disabled', false);

            $('.stepSizeRadioB').show();
            $('.stepSizeButtons').hide();

            $('#sldinstrStep').prop('checked', true);
            Stebs.ctrlState = Stebs.ctrlStates.instructionSteps;
        },
        macroSteps(): void {
            Stebs.ctrlStates.instructionSteps();
        },
        microSteps(): void {
            Stebs.ctrlStates.instructionSteps();
        }
    };
    export var ctrlState = ctrlStates.start;

    export 

    var ctx: CanvasRenderingContext2D;
    var canvas: HTMLCanvasElement;

    export var instructions: any;

    /**
     * The clientHub is a public singleton object, which contains client methods that can be called by the SignalR server.
     */
    export var clientHub = {

        /**
         * Receive available assembly instructions from the server.
         * TODO: Add type to data.
         */
        instructions(data: any): void {
            Stebs.instructions = data;
            //Simplify input for syntax highlighting
            for (var instruction in data) {
                assemblerInstruction[data[instruction].Mnemonic] = 'variable-2';
            }
        }

    };

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
         * Sets the width of #codingFrame to a prozentual value.
         * This allows correct browser resizing without additional client code.
         */
        setCodingFrameHeight(): void {
            var height = (visible.output ? ' - ' + heights.containerSize : '') + (visible.ram ? ' - ' + heights.containerSize : '');
            $('#codingFrame').css('height', 'calc(100% - ' + heights.bars + ' - ' + heights.runAndDebug + height + ')');
        },

        /**
         * Opens/Closes the ram bar.
         */
        toggleRAM(): void {
            $('#codingFrame').animate({ height: (visible.ram ? '+=' : '-=') + heights.containerSize }, ui.setCodingFrameHeight);
            visible.ram = !visible.ram;
            if (visible.ram) { $('.ram-container').slideDown(); }
            else { $('.ram-container').slideUp(); }
        },

        /**
         * Opens/Closes the output bar.
         */
        toggleOutput(): void {
            $('#codingFrame').animate({ height: (visible.output ? '+=' : '-=') + heights.containerSize }, ui.setCodingFrameHeight);
            visible.output = !visible.output;
            if (visible.output) { $('.output-container').slideDown(); }
            else { $('.output-container').slideUp(); }
        },

        openOutput(): void {
            if (!visible.output) { this.toggleOutput(); }
        },

        setOutput(text: string): void {
            $('#outputText').text(text);
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

/**
 * Import of the javascript global variable from mode.assembler.js
 */
declare var assemblerInstruction: any;

$(document).ready(function (){
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

    hub.client.assembled = function (result: string) {
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

    $('#btnPauseRun').click(function () {
        if (Stebs.ctrlState == Stebs.ctrlStates.debuggingAndPause || Stebs.ctrlState == Stebs.ctrlStates.assembled) {
            Stebs.ctrlStates.instructionSteps();
        } else {
            Stebs.ctrlStates.debuggingAndPause();
        }
    });

    $('#btnStop').click(function () {
        Stebs.ctrlStates.debuggingAndPause();
    });

    var editor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#editableTxtArea').get(0), {
        lineNumbers: true,
        mode: 'assembler'
    });

    Stebs.ctrlStates.start();
});
