/// <reference path="ram.ts"/>

module Stebs {
    export var visible = {
        devices: false,
        architecture: false,
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
        /* 1x topbar & 1x containerBar */
        bars: '76px',
        runAndDebug: '100px'
    };

    export enum SimulationStepSize { Micro = 0, Macro = 1, Instruction = 2 };

    export var utility = {
        addLeadingZeros(value: number, radix: number, size: number): string {
            return (Array(size + 1).join('0') + value.toString(radix)).substr(-size);
        }
    };

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
            ramContent.setContent(ram);
            ramContent.setRam2Line(code2Line);
            state.assembled();
        },

        /**
         * The sent source contains syntax errors. The assembling failed.
         */
        assembleError(error: string): void {
            ui.openOutput();
            ui.showOutput(error);
        },

        /**
        * Save the created file ID.
        */
        setFileId(id: number): void {
            var node = fileManagement.actualNode.getById(-1);
            console.log("node is " + node);
            if (node != null) {
                node.setId(id);
            }
        },

        fileContent(fileContent: string) {
            Stebs.codeEditor.getDoc().setValue(fileContent);
        },

        /**
        * Add all available registers.
        */
        registers(registers: string[]) {
            registerControl.addAll(registers);
        },

        /**
         * Update ram and register with sent updates.
         */
        updateProcessor(stepSize: SimulationStepSize, ramChanges: { [address: number]: number }, registerChanges: { [register: string]: { Type: number, Value: number } }) {
            Stebs.ramContent.resetHighlights();
            Stebs.watchControl.resetHighlighting();
            for (var address in ramChanges) {
                ramContent.setRamAt(address, ramChanges[address]);
            }
            for (var register in registerChanges) {
                registerControl.updateRegister(register, registerChanges[register].Value);
            }
        },

        /**
         * Called, when the processor was soft resetted.
         * (All registers cleared, but memory unchanged.)
         */
        reset() {
            registerControl.resetRegisters();
        }

    };

    export var serverHub = {

        /**
         * Sends the source to the server to be assembled.
         */
        assemble() {
            var newSource = Stebs.codeEditor.getDoc().getValue().replace(/\r?\n/g, '\r\n').replace(/\t/g, '    ');
            $.connection.stebsHub.server.assemble(newSource);
        },

        /**
         * Sends a request for a simulation step with given step size to the server.
         */
        singleStep(stepSize: SimulationStepSize) {
            $.connection.stebsHub.server.step(stepSize);
        },

        /**
         * Starts the simulation of the processor.
         */
        run(stepSize: SimulationStepSize) {
            $.connection.stebsHub.server.run(stepSize);
        },

        /**
         * Pauses the simulation of the server.
         * Simulation can be continued with another call to run.
         */
        pause() {
            $.connection.stebsHub.server.pause();
        },

        /**
         * Stops the simulation of the processor.
         */
        stop() {
            $.connection.stebsHub.server.stop();
        },

        /**
         * Changes the simulation speed: The speed is used as minimal delay between two simulation steps.
         */
        changeSpeed(speed: number) {
            $.connection.stebsHub.server.changeRunDelay(speed);
        },

        /**
         * Changes the simulation step size. (Rsolution of the running animation.)
         */
        changeStepSize(stepSize: SimulationStepSize) {
            $.connection.stebsHub.server.changeStepSize(stepSize);
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
            var height = (visible.output ? ' - ' + heights.containerSize : '');
            $('#codingFrame').css('height', 'calc(100% - ' + heights.bars + ' - ' + heights.runAndDebug + height + ')');
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
            outputView.getDoc().setValue(text);
        },

        /**
         * Reads the selected step size from the radio buttons.
         */
        getStepSize(): SimulationStepSize {
            if ($('#instructionStepSpeed').prop('checked')) { return SimulationStepSize.Instruction; }
            else if ($('#macroStepSpeed').prop('checked')) { return SimulationStepSize.Macro; }
            else { return SimulationStepSize.Micro; }
        }

    };

    /**
     * This interface allows the usage of the CodeMirror library.
     */
    export interface CodeMirror {
        codeMirror: any;
    };

    export var codeEditor: CodeMirror.EditorFromTextArea;
    export var outputView: CodeMirror.EditorFromTextArea;
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

    var falseDelegate = (delegate: () => void) => function () { delegate(); return false; };

    Stebs.ui.setupCanvas();
    Stebs.ramContent.init();
    Stebs.stateInit();

    var hub = $.connection.stebsHub;
    hub.client.instructions = Stebs.clientHub.instructions;
    hub.client.assembled = Stebs.clientHub.assembled;
    hub.client.assembleError = Stebs.clientHub.assembleError;
    hub.client.setFileId = Stebs.clientHub.setFileId;
    hub.client.registers = Stebs.clientHub.registers;
    hub.client.updateProcessor = Stebs.clientHub.updateProcessor;
    hub.client.fileContent = Stebs.clientHub.fileContent;
    hub.client.reset = Stebs.clientHub.reset;

    $.connection.hub.start().done(function () {
        Stebs.fileManagement.init();
        Stebs.registerControl.init();

        //Get available assembly instructions
        hub.server.getInstructions();
        hub.server.getRegisters();

        Mousetrap.bindGlobal('ctrl+o', falseDelegate(Stebs.fileManagement.toggleFileManager));
        Mousetrap.bindGlobal('ctrl+n', falseDelegate(Stebs.fileManagement.newFile));
        Mousetrap.bindGlobal('ctrl+s', falseDelegate(() => console.log('save called'))); //TODO: implement

        $('#assemble').click(() => Stebs.state.assemble());
        Mousetrap.bindGlobal('ctrl+b', falseDelegate(() => Stebs.state.assemble()));

        $('#debug').click(() => Stebs.state.debug());
        Mousetrap.bindGlobal('ctrl+j', falseDelegate(() => Stebs.state.debug()));

        $('#start').click(() => Stebs.state.start());
        $('#pause, #continue').click(() => Stebs.state.startOrPause());
        Mousetrap.bind('space', falseDelegate(() => Stebs.state.startOrPause()));
        Mousetrap.bindGlobal('ctrl+g', falseDelegate(() => Stebs.state.startOrPause()));

        $('#stop').click(() => Stebs.state.stop());
        Mousetrap.bindGlobal(['esc', 'ctrl+h'], falseDelegate(() => Stebs.state.stop()));

        $('#instructionStep').click(() => Stebs.state.singleStep(Stebs.SimulationStepSize.Instruction));
        $('#macroStep').click(() => Stebs.state.singleStep(Stebs.SimulationStepSize.Macro));
        $('#microStep').click(() => Stebs.state.singleStep(Stebs.SimulationStepSize.Micro));

        $('#speedSlider').change(() => {
            Stebs.serverHub.changeSpeed((2000 + 10) - parseInt($('#speedSlider').val()))
        });
        $('.stepSizeRadios input').change(() => Stebs.serverHub.changeStepSize(Stebs.ui.getStepSize()));
    });

    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $('#openOutput').click(Stebs.ui.toggleOutput);

    Stebs.codeEditor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#codingTextArea').get(0), {
        lineNumbers: true,
        mode: 'assembler'
    });
    Stebs.outputView = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#outputTextArea').get(0), {
        lineNumbers: true,
        mode: 'assembler',
        readOnly: true,
        cursorBlinkRate: -1
    });

});
