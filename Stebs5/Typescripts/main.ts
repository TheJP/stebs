/// <reference path="ram.ts"/>

module Stebs {
    export var visible = {
        devices: false,
        architecture: false,
        output: false
    };

    export var widths = {
        devices: '350px',
        architecture: '400px',
        sidebar: '30px'
    };

    export var heights = {
        topbar: '38px',
        output: '150px',
        /* 1x topbar & 1x containerBar */
        //containerBar: '25px',
        bars: '63px',
        runAndDebug: '110px'
    };

    export enum SimulationStepSize { Micro = 0, Macro = 1, Instruction = 2 };

    export function convertNumber(value: number, radix: number, size: number): string {
        return (Array(size + 1).join('0') + value.toString(radix)).substr(-size);
    };

    /**
     * The clientHub is a public singleton object, which contains client methods that can be called by the SignalR server.
     */
    export var clientHub = {

        /**
         * Receive available assembly instructions from the server.
         */
        init(data: any): void {
            //Add syntax highlighting for received instructions
            for (var instruction in data.Instructions) {
                assemblerInstruction[data.Instructions[instruction].Mnemonic] = 'variable-2';
            }
            //Initialise components
            registerControl.addAll(data.Registers);
            deviceManager.setDeviceTypes(data.DeviceTypes);
            //Add processor id to the ram download link
            var link = $('#downloadRam');
            link.prop('href', link.prop('href') + '?processorId=' + data.ProcessorId);
        },

        /**
         * Server finished assembling the sent source.
         */
        assembled(result: string, ram: number[], code2Line: number[]): void {
            Stebs.outputView.setOption('mode', 'assembler');
            ui.openOutput();
            ui.showOutput(result);
            ramContent.setContent(ram);
            ramContent.setRamToLine(code2Line);
            state.assembled();
        },

        /**
         * The sent source contains syntax errors. The assembling failed.
         */
        assembleError(error: string): void {
            Stebs.outputView.setOption('mode', 'none');
            ui.openOutput();
            ui.showOutput(error);
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
            Stebs.ui.highlightLine(registerControl.registers['IP'].getValue());
        },

        /**
         * Update interrupt flag (IRF) with the sent update.
         */
        processorInterrupt(flagValue: number): void {
            registerControl.updateRegister('Interrupt', flagValue);
        },

        /**
         * Called, when the processor was soft resetted.
         * (All registers cleared, but memory unchanged.)
         */
        reset() {
            registerControl.resetRegisters();
        },

        /**
         * Called, when the processor was hard resetted.
         * (All registers and complete memory cleared.)
         */
        hardReset() {
            registerControl.resetRegisters();
            ramContent = new Ram();
            ramContent.init();
        },

        /**
         * Called, when the processor was halted.
         */
        halt() {
            state.halted();
        }

    };

    export class AddDeviceViewModel {
        public Slot: number;
        public Template: string;
        public Success: boolean;
    };

    export class RemoveDeviceViewModel {
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
         * Stops the simulation of the processor and resets the ram and all registers.
         */
        reset() {
            $.connection.stebsHub.server.reset();
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
        },

        /*
        * Add a Node to the Filesystem
        */
        addNode(parentId: number, nodeName: string, isFolder: boolean): Promise<FileSystem> {
            return $.connection.stebsHub.server.addNode(parentId, nodeName, isFolder);
        },

        /**
        * Change Node name
        */
        changeNodeName(nodeId: number, newNodeName: string, isFolder: boolean): Promise<FileSystem> {
            return $.connection.stebsHub.server.changeNodeName(nodeId, newNodeName, isFolder);
        },

        /**
        * Delete Node 
        */
        deleteNode(nodeId: number, isFolder: boolean): Promise<FileSystem> {
            return $.connection.stebsHub.server.deleteNode(nodeId, isFolder);
        },

        /**
        * Get Filesystem 
        */
        getFileSystem(): Promise<FileSystem> {
            return $.connection.stebsHub.server.getFileSystem();
        },

        /**
        * Get File content.
        */
        getFileContent(nodeId: number): Promise<string> {
            return $.connection.stebsHub.server.getFileContent(nodeId);
        },

        /**
        * Save File content.
        */
        saveFileContent(nodeId: number, fileContent: string): void {
            $.connection.stebsHub.server.saveFileContent(nodeId, fileContent);
        },

        /**
         * Add a new device with the given type at the given slot.
         * @param deviceType Device id, which should be added.
         * @param slot Prefered slot number.
         */
        addDevice(deviceType: string, slot: number = NaN): Promise<AddDeviceViewModel> {
            return $.connection.stebsHub.server.addDevice(deviceType, isNaN(slot) ? null : slot);
        },

        /**
         * Updates a device with user input.
         * @param slot Slot number of the device to update.
         * @param update Update data from client to server.
         */
        updateDevice(slot: number, update: any): void {
            $.connection.stebsHub.server.updateDevice(slot, update);
        },

        /**
         * Removes the device with the given slot.
         */
        removeDevice(slot: number): Promise<RemoveDeviceViewModel> {
            return $.connection.stebsHub.server.removeDevice(slot);
        }

    };

    export var codeEditor: CodeMirror.EditorFromTextArea;
    export var outputView: CodeMirror.EditorFromTextArea;
}

/**
 * This interface allows the usage of the signalr library.
 */
interface JQueryStatic {
    connection: {
        stebsHub: { server: any, client: any },
        hub: any
    };
}

/**
 * This interface allows the usage of the bindGlobal methods.
 * These allow definitions of keybindings, which also work in the code mirror editor.
 */
interface MousetrapStatic {
    bindGlobal(keys: string, callback: (e: ExtendedKeyboardEvent, combo: string) => any, action?: string): void;
    bindGlobal(keyArray: string[], callback: (e: ExtendedKeyboardEvent, combo: string) => any, action?: string): void;
}

module CodeMirror {
    export interface EditorConfiguration {
        styleActiveLine?: boolean;
    }
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
    hub.client.assembled = Stebs.clientHub.assembled;
    hub.client.assembleError = Stebs.clientHub.assembleError;
    hub.client.updateProcessor = Stebs.clientHub.updateProcessor;
    hub.client.processorInterrupt = Stebs.clientHub.processorInterrupt;
    hub.client.reset = Stebs.clientHub.reset;
    hub.client.halt = Stebs.clientHub.halt;
    hub.client.hardReset = Stebs.clientHub.hardReset;
    hub.client.updateDevice = Stebs.deviceManager.updateView;

    $.connection.hub.start().done(function () {

        //Initialise stebs
        hub.server.initialise().done(Stebs.clientHub.init);
        Stebs.fileManagement.init();
        Stebs.registerControl.init();
        Stebs.deviceManager.init();

        Mousetrap.bindGlobal('mod+o', falseDelegate(Stebs.fileManagement.toggleFileManager));
        Mousetrap.bindGlobal('mod+n', falseDelegate(Stebs.fileManagement.newFile));
        Mousetrap.bindGlobal('mod+s', falseDelegate(Stebs.fileManagement.saveFile));

        $('#assemble').click(() => Stebs.state.assemble());
        Mousetrap.bindGlobal('mod+b', falseDelegate(() => Stebs.state.assemble()));

        $('#debug').click(() => Stebs.state.debug());
        Mousetrap.bindGlobal('mod+j', falseDelegate(() => Stebs.state.debug()));

        $('#start').click(() => Stebs.state.start());
        $('#pause, #continue').click(() => Stebs.state.startOrPause());
        Mousetrap.bind('space', falseDelegate(() => Stebs.state.startOrPause()));
        Mousetrap.bindGlobal('mod+g', falseDelegate(() => Stebs.state.startOrPause()));

        $('#stop').click(() => Stebs.state.stop());
        Mousetrap.bindGlobal(['esc', 'mod+h'], falseDelegate(() => Stebs.state.stop()));

        $('#reset').click(() => Stebs.state.reset());
        //TODO: Add keyboard binding

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
        mode: 'assembler',
        lineNumbers: true,
        styleActiveLine: true
    });
    Stebs.outputView = CodeMirror.fromTextArea(<HTMLTextAreaElement>$('#outputTextArea').get(0), {
        mode: 'assembler',
        lineNumbers: true,
        readOnly: true,
        cursorBlinkRate: -1
    });

    //Get change event from codeEditor
    Stebs.codeEditor.on("change", function (cm, change) {
        Stebs.state.codeChanged();
        Stebs.ui.setEditorContentChanged(true);
    })

    //Show confirm. If the user stays on page the connection will be recreated 
    $(window).on('beforeunload', function () {
        var timeout: number;
        if (Stebs.ui.isEditorContentChanged()) {
            timeout = setTimeout(function () {
                //Reconnect
                $.connection.hub.start();
                //Because processor was deleted on disconnect
                Stebs.stateInit(false);
                Stebs.ramContent = new Stebs.Ram();
                Stebs.ramContent.init();
                Stebs.registerControl.resetRegisters();
            }, 1000);
            return 'Are you sure you want to leave?';
        }
    });


});
