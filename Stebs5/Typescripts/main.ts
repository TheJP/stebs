/// <reference path="ram.ts"/>

module Stebs {
    export var visible = {
        devices: false,
        architecture: false,
        ram: false,
        output: false
    };

    export var widths = {
        devices: '350px',
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

    export var utility = {
        addLeadingZeros(value: number, radix: number, size: number): string {
            return (Array(size + 1).join('0') + value.toString(radix)).substr(-size);
        }
    };

    export var controlStates = {
        start(): void {
            $('#debug img').attr('src', 'Icons/Debug-icon-grey.png');
            $('#run img').attr('src', 'Icons/Play-icon-grey.png');
            $('#pauseRun img').attr('src', 'Icons/Pause-icon-grey.png');
            $('#stop img').attr('src', 'Icons/Stop-icon-grey.png');

            $('#debug').prop('disabled', true);
            $('#run').prop('disabled', true);
            $('#pauseRun').prop('disabled', true);
            $('#stop').prop('disabled', true);

            $('.stepSizeRadios').hide();
            $('.stepSizeButtons').show();
            controlState = controlStates.start;
        },
        stop(): void {
            $('#debug img').attr('src', 'Icons/Debug-icon.png');
            $('#run img').attr('src', 'Icons/Play-icon.png');
            $('#pauseRun img').attr('src', 'Icons/Play-icon.png');
            $('#stop img').attr('src', 'Icons/Stop-icon-grey.png');

            $('#debug').prop('disabled', false);
            $('#run').prop('disabled', false);
            $('#pauseRun').prop('disabled', false);
            $('#stop').prop('disabled', true);

            controlState = controlStates.stop;
        },
        assembled(): void {
            $('#debug img').attr('src', 'Icons/Debug-icon.png');
            $('#run img').attr('src', 'Icons/Play-icon.png');
            $('#pauseRun img').attr('src', 'Icons/Play-icon.png');
            $('#stop img').attr('src', 'Icons/Stop-icon-grey.png');

            $('#debug').prop('disabled', false);
            $('#run').prop('disabled', false);
            $('#pauseRun').prop('disabled', false);
            $('#stop').prop('disabled', true);

            $('.stepSizeRadios').hide();
            $('.stepSizeButtons').show();
            controlState = controlStates.assembled;
        },
        debuggingAndPause(): void {
            $('#debug img').attr('src', 'Icons/Debug-icon.png');
            $('#run img').attr('src', 'Icons/Play-icon.png');
            $('#pauseRun img').attr('src', 'Icons/Play-icon.png');
            $('#stop img').attr('src', 'Icons/Stop-icon.png');

            $('#debug').prop('disabled', false);
            $('#run').prop('disabled', false);
            $('#pauseRun').prop('disabled', false);
            $('#stop').prop('disabled', false);

            $('.stepSizeRadios').hide();
            $('.stepSizeButtons').show();
            controlState = controlStates.debuggingAndPause;
        },
        instructionSteps(): void {
            $('#debug img').attr('src', 'Icons/Debug-icon.png');
            $('#run img').attr('src', 'Icons/Play-icon.png');
            $('#pauseRun img').attr('src', 'Icons/Pause-icon.png');
            $('#stop img').attr('src', 'Icons/Stop-icon.png');

            $('#debug').prop('disabled', false);
            $('#run').prop('disabled', false);
            $('#pauseRun').prop('disabled', false);
            $('#stop').prop('disabled', false);

            $('.stepSizeRadios').show();
            $('.stepSizeButtons').hide();

            $('#instructionStepSpeed').prop('checked', true);
            controlState = controlStates.instructionSteps;
        },
        macroSteps(): void {
            controlStates.instructionSteps();
        },
        microSteps(): void {
            controlStates.instructionSteps();
        }
    };
    export var controlState = controlStates.start;

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
        },

        /**
         * Server finished assembling the sent source.
         */
        assembled(result: string, ram: number[], code2Line: number[]): void {
            ui.openOutput();
            ui.showOutput(result);
            Stebs.ramContent.setContent(ram);
            Stebs.ramContent.setRam2Line(code2Line);
            Stebs.controlStates.assembled();
        },

        /**
         * The sent source contains syntax errors. The assembling failed.
         */
        assembleError(error: string): void {
            ui.openOutput();
            ui.showOutput(error);
        },

        /**
        * Save the created file ID
        */
        setFileId(id: number): void {
            var node = Stebs.fileManagement.actualNode.getById(-1);
            console.log("node is " + node);
            if (node != null) {
                node.setId(id);
            }
        },

        /**
        * Add all available registers
        */
        setAvailableRegisters(registers: string[]) {
            Stebs.registerControl.addAll(registers);
            
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

        showOutput(text: string): void {
            var output = $('#outputText');
            output.text(text);
            output.html(output.html().replace(/\n/g, '<br>').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;').replace(/\s/g, '&nbsp;'));
        },

        loadRegisters(registerList: string[]) {

        }
    };

    export var ramContent = new Stebs.Ram(256);

    export var fileManagement = new Stebs.FileManagement();

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
 * This interface allows the usage of the bindGlobal methods.
 * These allow definitions of keybindings, which also work in the code mirror editor.
 */
interface MousetrapStatic {
    bindGlobal(keys: string, callback: (e: ExtendedKeyboardEvent, combo: string) => any, action?: string): void;
    bindGlobal(keyArray: string[], callback: (e: ExtendedKeyboardEvent, combo: string) => any, action?: string): void;
}

/**
 * Import of the javascript global variable from mode.assembler.js
 */
declare var assemblerInstruction: any;

$(document).ready(function () {
    Stebs.ui.setupCanvas();

    var hub = $.connection.stebsHub;
    hub.client.instructions = Stebs.clientHub.instructions;
    hub.client.assembled = Stebs.clientHub.assembled;
    hub.client.assembleError = Stebs.clientHub.assembleError;
    hub.client.setFileId = Stebs.clientHub.setFileId;
    hub.client.setAvailableRegisters = Stebs.clientHub.setAvailableRegisters;

    $.connection.hub.start().done(function () {
        //Get available assembly instructions
        hub.server.getInstructions();
        Stebs.fileManagement.init();
        Stebs.registerControl.init();
    });

    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $('#openRam').click(Stebs.ui.toggleRAM);
    $('#openOutput').click(Stebs.ui.toggleOutput);

    Stebs.ramContent.init();

    var assembleFunction = function () {
        var newSource = editor.getDoc().getValue().replace(/\r?\n/g, '\r\n');
        $.connection.stebsHub.server.assemble(newSource);
    };

    var pauseAndRunFunction = function () {
        if (Stebs.controlState == Stebs.controlStates.debuggingAndPause || Stebs.controlState == Stebs.controlStates.assembled || Stebs.controlState == Stebs.controlStates.stop) {
            Stebs.controlStates.instructionSteps();
        } else {
            Stebs.controlStates.debuggingAndPause();
        }
    }

    var falseDelegate = (delegate: () => void) => function () { delegate(); return false; };

    $('#assemble').click(assembleFunction);
    Mousetrap.bindGlobal('ctrl+b', falseDelegate(assembleFunction));

    $('#debug').click(Stebs.controlStates.debuggingAndPause);
    Mousetrap.bindGlobal('ctrl+d', falseDelegate(Stebs.controlStates.debuggingAndPause));

    $('#run').click(Stebs.controlStates.instructionSteps);
    $('#pauseRun').click(pauseAndRunFunction);
    Mousetrap.bind('space', falseDelegate(pauseAndRunFunction));
    Mousetrap.bindGlobal('ctrl+g', falseDelegate(pauseAndRunFunction));

    $('#stop').click(Stebs.controlStates.stop);
    Mousetrap.bindGlobal(['esc', 'ctrl+y'], falseDelegate(Stebs.controlStates.stop));

    var editor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#editableTxtArea').get(0), {
        lineNumbers: true,
        mode: 'assembler'
    });

    Stebs.controlStates.start();

});
