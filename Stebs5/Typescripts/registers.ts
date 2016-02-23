module Stebs {

    export var registerControl = {
        InitialStackPointer: 0xbf,

        registers: <{ [register: string]: Register }>{},
        defaultRegisters: ['AL', 'BL', 'CL', 'DL', 'IP', 'SP', 'Status'],
        propagateToRam: ['IP', 'SP'],

        /**
         * Initialize the registerControl
         */
        init(): void {
            $('#newWatchesButton').click(registerControl.showWatchSelection);
            $('#closeWatches').click(() => $('#addWatches').hide());
        },

        /**
         * Opens a dialog to select a new watch.
         */
        showWatchSelection() {
            var registers = registerControl.registersWithoutWatches();
            $('#elementList').empty();
            for (var i = 0; i < registers.length; i++) {
                var link = function () {
                    var register = registers[i];
                    return $('<a>')
                        .prop('href', '#')
                        .addClass('registerLink')
                        .text(register.getDisplayName())
                        .click(function () {
                            registerControl.addWatch(register.getType());
                            $('#addWatches').hide();
                        });
                } ();
                $('#elementList').append(link);
            }
            var offset = $('#newWatchesButton').offset();
            $('#addWatches').css('left', offset.left).css('bottom', Stebs.heights.runAndDebug);
            $('#addWatches').show();
        },

        /**
         * Called when available registers are received from the server.
         */
        setRegisters(registersByTypes: string[]) {
            //Remove all watches
            for (var type in this.registers) {
                var register: Register = this.registers[type];
                if (register.hasWatchElemet()) { register.getWatchElement().remove(); }
            }
            //Remove all registers
            registerControl.registers = {};
            //Add new register and watches
            for (var i = 0; i < registersByTypes.length; i++) {
                var newRegister = new Register(registersByTypes[i]);
                this.registers[registersByTypes[i]] = newRegister;
                if (registerControl.defaultRegisters.indexOf(registersByTypes[i]) != -1) {
                    newRegister.addWatchElement();
                }
            }
            registerControl.resetRegisters();
        },

        /**
         * Update the register (type) to the given value.
         * @param type the register to change.
         * @param value the new calue.
         */
        updateRegister(type: string, value: number) {
            if (type in registerControl.registers) {
                registerControl.registers[type].updateValue(value);
                if (type == 'IP') {
                    ramContent.setInstructionPointer(value);
                } else if (type == 'SP') {
                    ramContent.setStackPointer(value);
                }
            }
        },

        /** 
         * Resets all registers to their initial value.
         */
        resetRegisters() {
            for (var type in registerControl.registers) {
                registerControl.registers[type].reset();
            }
            watchControl.resetHighlighting();
            ramContent.setInstructionPointer(0);
            ramContent.setStackPointer(registerControl.InitialStackPointer);
        },
        /**
         * Add a watch element to the register (type).
         * @param type the register to add a watch.
         */
        addWatch(type: string) {
            if (type in registerControl.registers) {
                registerControl.registers[type].addWatchElement();
            }
        },

        /**
         * Returns an array, which contains all register types that don't have a watch yet.
         */
        registersWithoutWatches(): Register[] {
            var registerNames: Register[] = [];
            for (var type in registerControl.registers) {
                var register = registerControl.registers[type];
                if (!register.hasWatchElemet()) { registerNames.push(register); }
            }
            return registerNames;
        }

    };
    
    export var watchControl = {

        resetHighlighting(): void {
            $('.watcher').removeClass('changed');
        }

    };

    export class Register {
        /**
        * Convert a type to a replacement name.
        */
        private static typeToName: { [type: string]: string } = {
            ['Interrupt']: 'IRF', //IRF = Interrupt Flag
            ['Status']: 'SR' //SR = Status Register
        };

        /**
        * Create a StatusWatchElement form a register.
        */
        private static watchFactories: { [type: string]: (register: Register) => WatchElement } = {
            ['Status']: (register) => new StatusWatchElement(register),
            ['MIP']: (register) => new MipWatchElement(register),
            ['Interrupt']: (register) => new IrfWatchElement(register)
        }

        private type: string;
        private value: number;
        private watchElement: WatchElement;

        constructor(name: string) {
            this.type = name;
            this.value = 0;
        }

        public getType(): string {
            return this.type;
        }

        public getValue(): number {
            return this.value;
        }

        public getWatchElement(): WatchElement {
            return this.watchElement;
        }

        public hasWatchElemet(): boolean {
            return this.watchElement != null;
        }

        /**
         * Update the register to the newValue.
         * @param newValue the newValue.
         */
        public updateValue(newValue: number) {
            this.value = newValue;
            if (this.watchElement != null) {
                this.watchElement.changed();
            }
        }

        /** 
        * Resets the value of this register to the initial state. 
        */
        public reset() {
            if (this.getType() == 'SP') { this.updateValue(registerControl.InitialStackPointer); }
            else { this.updateValue(0); }
        }

        /**
         * Add a watchelement.
         */
        public addWatchElement() {
            if (this.getType() in Register.watchFactories) { this.watchElement = Register.watchFactories[this.getType()](this); }
            else { this.watchElement = new WatchElement(this); }
            this.watchElement.show();
        }

        /**
         * Remove the watchElement
         */
        public removeWatchElement() {
            this.watchElement = null;
        }

        /**
        * Returns the name, which should be displayed for this watch element.
        */
        getDisplayName(): string {
            return this.getType() in Register.typeToName ? Register.typeToName[this.getType()] : this.getType()
        }
    };

    export class WatchElement {
        private register: Register;
        private showBinary: boolean = false;

        constructor(register: Register) {
            this.register = register;
        }

        getRegister(): Register {
            return this.register;
        }

        getType(): string {
            return this.getRegister().getType();
        }

        isShowBinary(): boolean {
            return this.showBinary;
        }

        setShowBinary(value: boolean) {
            this.showBinary = value;
            this.update();
        }

        /** Toggles element between binary and hex representation. */
        toggleRepresentation(): void { this.setShowBinary(!this.isShowBinary()); }

        /** Adds a new watch of this register type to the watcher elements. */
        show(): void {
            $('#watcher-elements').append(this.asJQuery());
            this.update();
        }

        /** Removes this watch from the watcher elements. */
        remove(): void {
            $('#watch-' + this.getType()).remove();
            this.getRegister().removeWatchElement();
        }

        /** Updates the view of this watcher element. */
        update(): void {
            $('#watch-' + this.getType() + ' .watch-element-value').text(this.getValueFormated());
        }

        changed(): void {
            $('#watch-' + this.getType()).addClass('changed');
            this.update();
        }

        /**
         * Get the value, which should be shown for this watch element.
         * The output depends on the specified number base setting (hex or binary).
         */
        getValueFormated(): string {
            if (this.showBinary) {
                var binary = convertNumber(this.register.getValue(), 2, 8);
                return binary.slice(0, 4) + '\'' + binary.slice(4, 8);
            }
            return convertNumber(this.register.getValue(), 16, 2);
        }

        /** Creates the html structure for this watch. */
        asJQuery(): JQuery {
            var name = $('<p>').text(this.getRegister().getDisplayName());
            var link = $('<a>')
                .prop('href', '#')
                .addClass('watch-element-value')
                .text(this.getValueFormated())
                .click(() => this.toggleRepresentation());
            var closeButton = $('<button>')
                .text('x')
                .click(() => this.remove());
            return $('<div>')
                .prop('id', 'watch-' + this.getType())
                .addClass('watcher')
                .append(closeButton)
                .append(name)
                .append(link);
        }

    }

    enum CustomWatchStates { Custom, Hex, Bin };

    /**
     * Base class for whatch elements that toggle between 3 states.
     */
    abstract class CustomStateWhatchElement extends WatchElement {

        private state = CustomWatchStates.Custom

        getState(): CustomWatchStates {
            return this.state;
        }

        /** Toggles between the three possible states. */
        toggleRepresentation() {
            if (this.state == CustomWatchStates.Custom) { this.state = CustomWatchStates.Bin; this.setShowBinary(true); }
            else if (this.state == CustomWatchStates.Bin) { this.state = CustomWatchStates.Hex; this.setShowBinary(false); }
            else { this.state = CustomWatchStates.Custom; this.setShowBinary(false); }
        }
    }

    class StatusWatchElement extends CustomStateWhatchElement {

        update() {
            if (this.getState() != CustomWatchStates.Custom) { super.update(); return; }
            var value = this.getRegister().getValue();
            var interrupt = (value & 16) > 0 ? 1 : 0;
            var signed = (value & 8) > 0 ? 1 : 0;
            var overflow = (value & 4) > 0 ? 1 : 0;
            var zero = (value & 2) > 0 ? 1 : 0;
            $('#watch-' + this.getType() + ' .watch-element-value').text('I:' + interrupt + ' S:' + signed + ' O:' + overflow + ' Z:' + zero);
        }

    }

    class IrfWatchElement extends CustomStateWhatchElement {

        update() {
            if (this.getState() != CustomWatchStates.Custom) { super.update(); return; }
            $('#watch-' + this.getType() + ' .watch-element-value').text('IRF: ' + this.getRegister().getValue());
        }

    }

    class MipWatchElement extends WatchElement {

        getValueFormated(): string {
            if (this.isShowBinary()) {
                var binary = convertNumber(this.getRegister().getValue(), 2, 12);
                return binary.slice(0, 4) + '\'' + binary.slice(4, 8) + '\'' + binary.slice(8, 12);
            }
            return convertNumber(this.getRegister().getValue(), 16, 3);
        }

    }
    
}