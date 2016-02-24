/// <reference path="ram.ts"/>

module Stebs {

    export var visible = {
        devices: false,
        architecture: false,
        output: false
    };

    export var Widths = {
        devices: '350px',
        architecture: '400px'
    };

    export var Heights = {
        topbar: '38px',
        containerBar: '38px',
        containerSize: '150px',
        /* 1x topbar & 1x containerBar */
        bars: '76px',
        runAndDebug: '100px'
    };

    /** Speed slider values in ms. */
    var SpeedSlider = {
        min: 10,
        max: 5000
    };

    export enum SimulationStepSize { Micro = 0, Macro = 1, Instruction = 2 };

    export function convertNumber(value: number, radix: number, size: number): string {
        return (Array(size + 1).join('0') + value.toString(radix)).substr(-size);
    };

    var ctx: CanvasRenderingContext2D;
    var canvas: HTMLCanvasElement;

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
            registerControl.setRegisters(data.Registers);
            deviceManager.setDeviceTypes(data.DeviceTypes);
            //Add processor id to the ram download link
            ui.setRamDownloadLink(data.ProcessorId);
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

    export var ui = {

        private editorContentChanged: false,

        /**
         * Sets the flag, if stebs thinks the editor content is changed.
         */
        setEditorContentChanged(value: boolean) {
            ui.editorContentChanged = value;
            $('#filename-star').css('display', value ? 'inline' : 'none');
        },

        /**
         * Returns if the editor content is flaged as changed.
         */
        isEditorContentChanged(): boolean {
            return ui.editorContentChanged;
        },

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
            var width = (visible.architecture ? ' - ' + Widths.architecture : '') + (visible.devices ? ' - ' + Widths.devices : '');
            $('#codingView').css('width', 'calc(100% - 50px' + width + ')');
        },

        /**
         * Opens/Closes the devices sidebar.
         */
        toggleDevices(): void {
            var animation = { left: (visible.devices ? '-=' : '+=') + Widths.devices };
            $('#devices, #architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.devices ? '+=' : '-=') + Widths.devices };
            $('#codingView').animate(animation2, ui.setCodingViewWidth);
            visible.devices = !visible.devices;
        },

        /**
         * Opens/Closes the architecture sidebar.
         */
        toggleArchitecture(): void {
            var animation = { left: (visible.architecture ? '-=' : '+=') + Widths.architecture };
            $('#architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.architecture ? '+=' : '-=') + Widths.architecture };
            $('#codingView').animate(animation2, ui.setCodingViewWidth);
            visible.architecture = !visible.architecture;
        },

        /**
         * Sets the width of #codingFrame to a prozentual value.
         * This allows correct browser resizing without additional client code.
         */
        setCodingFrameHeight(): void {
            var height = (visible.output ? ' - ' + Heights.containerSize : '');
            $('#codingFrame').css('height', 'calc(100% - ' + Heights.bars + ' - ' + Heights.runAndDebug + height + ')');
        },

        /**
         * Opens/Closes the output bar.
         */
        toggleOutput(): void {
            $('#codingFrame').animate({ height: (visible.output ? '+=' : '-=') + Heights.containerSize }, ui.setCodingFrameHeight);
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
        },

        /**
        * Highlight the given line
        */
        highlightLine(ipNr: number) {
            var lineNummer = Stebs.ramContent.getLineNr(ipNr);
            Stebs.codeEditor.getDoc().setCursor({ ch: 0, line: lineNummer });
        },

        /**
         * Sets the ram download link using the new processor id.
         */
        setRamDownloadLink(processorId: string) {
            var link = $('#downloadRam');
            var href: string = link.prop('href');
            var position = href.indexOf('?processorId');
            if (position >= 0) { href = href.substring(0, position); }
            link.prop('href', href + '?processorId=' + processorId);
        },

        /**
         * Allows the use of a logarithmic slider.
         */
        logarithmicScale(value: number, min: number, max: number, inverse: boolean) {
            //position will be between 0 and 100 (fixed)
            var maxPosition = 100;
            //The result should be between given min an max
            var minValue = Math.log(min);
            var maxValue = Math.log(max);
            //calculate adjustment factor
            var scale = (maxValue - minValue) / (maxPosition);
            if (inverse) { return (Math.log(value) - minValue) / scale; }
            else { return Math.exp(minValue + scale * (value)); }
        },

        /**
         * Returns the position of the slider [0, 100] calculated using a logarithmic function.
         */
        logarithmicPosition(value: number, min: number, max: number) {
            return ui.logarithmicScale(value, min, max, true);
        },

        /**
         * Returns the value of a slider [0, 100] scaled to a logarithmic function.
         */
        logarithmicValue(position: number, min: number, max: number): number {
            return ui.logarithmicScale(position, min, max, false);
        },

        /**
         * Is Called, when the value of the speed slider changed.
         */
        speedSliderChanged() {
            var newValue = ui.logarithmicValue(parseInt($('#speedSlider').val()), SpeedSlider.max, SpeedSlider.min);
            Stebs.serverHub.changeSpeed(newValue);
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
    hub.client.initialise = Stebs.clientHub.init;
    hub.client.assembled = Stebs.clientHub.assembled;
    hub.client.assembleError = Stebs.clientHub.assembleError;
    hub.client.updateProcessor = Stebs.clientHub.updateProcessor;
    hub.client.processorInterrupt = Stebs.clientHub.processorInterrupt;
    hub.client.reset = Stebs.clientHub.reset;
    hub.client.halt = Stebs.clientHub.halt;
    hub.client.hardReset = Stebs.clientHub.hardReset;
    hub.client.updateDevice = Stebs.deviceManager.updateView;

    $.connection.hub.start().done(() => {

        //Initialise stebs
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

        $('#speedSlider').change(Stebs.ui.speedSliderChanged);
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
            //Only reconnect if the browser is firefox.
            //The false disconnection bug is known and intentionally not fixed by the SignalR developers.
            if (navigator.userAgent.toLowerCase().indexOf('firefox') >= 0) {
                timeout = setTimeout(function () {
                    //Reconnect
                    $.connection.hub.start().done(() => {
                        //Reinitialise devices
                        Stebs.deviceManager.reInitialise();
                    });
                    //Because processor was deleted on disconnect
                    Stebs.stateInit(false);
                    Stebs.ramContent = new Stebs.Ram();
                    Stebs.ramContent.init();
                    Stebs.registerControl.resetRegisters();
                }, 1000);
            }
            return 'Are you sure you want to leave?';
        }
    });


});
