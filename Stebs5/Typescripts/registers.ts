module Stebs {

    export var registerControl = {

        registers: <{ [register: string]: Register }>{},
        defaultRegisters: ['AL', 'BL', 'CL', 'DL', 'IP', 'SP'],
        propagateToRam: ['IP', 'SP'],

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
        addAll(registersByTypes: string[]) {
            for (var i = 0; i < registersByTypes.length; i++) {
                var newRegister = new Register(registersByTypes[i]);
                this.registers[registersByTypes[i]] = newRegister;
                if (registerControl.defaultRegisters.indexOf(registersByTypes[i]) != -1) {
                    newRegister.addWatchElement();
                }
            }
            registerControl.resetRegisters();
        },

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
        },

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

        private static typeToName: { [type: string]: string } = {
            ['Interrupt']: 'IRF', //IRF = Interrupt Flag
            ['Status']: 'SR' //SR = Status Register
        };
        private static watchFactories: { [type: string]: (register: Register) => WatchElement } = {
            ['Status']: (register) => new StatusWatchElement(register)
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

        public hasWatchElemet(): boolean {
            return this.watchElement != null;
        }

        public updateValue(newValue: number) {
            this.value = newValue;
            if (this.watchElement != null) {
                this.watchElement.changed();
            }
        }

        /** Resets the value of this register to the initial state. */
        public reset() {
            if (this.getType() == 'SP') { this.updateValue(0xbf); }
            else { this.updateValue(0); }
        }

        public addWatchElement() {
            if (this.getType() in Register.watchFactories) { this.watchElement = Register.watchFactories[this.getType()](this); }
            else { this.watchElement = new WatchElement(this); }
            this.watchElement.show();
        }

        public removeWatchElement() {
            this.watchElement = null;
        }

        /** Returns the name, which should be displayed for this watch element. */
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
                var asBinay = utility.addLeadingZeros(this.register.getValue(), 2, 8);
                var asBinaryValue = asBinay.slice(0, 4);
                asBinaryValue += '\'';
                asBinaryValue += asBinay.slice(4, 8);
                return asBinaryValue;
            }
            return utility.addLeadingZeros(this.register.getValue(), 16, 2);
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

    enum StatusWatchStates { Custom, Hex, Bin };

    class StatusWatchElement extends WatchElement {

        private state = StatusWatchStates.Custom

        /** Toggles between the three possible states. */
        toggleRepresentation() {
            if (this.state == StatusWatchStates.Custom) { this.state = StatusWatchStates.Bin; this.setShowBinary(true); }
            else if (this.state == StatusWatchStates.Bin) { this.state = StatusWatchStates.Hex; this.setShowBinary(false); }
            else { this.state = StatusWatchStates.Custom; this.setShowBinary(false); }
        }

        update() {
            if (this.state != StatusWatchStates.Custom) { super.update(); return; }
            var value = this.getRegister().getValue();
            var interrupt = (value & 16) > 0 ? 1 : 0;
            var signed = (value & 8) > 0 ? 1 : 0;
            var overflow = (value & 4) > 0 ? 1 : 0;
            var zero = (value & 2) > 0 ? 1 : 0;
            $('#watch-' + this.getType() + ' .watch-element-value').text('I:' + interrupt + ' S:' + signed + ' O:' + overflow + ' Z:' + zero);
        }
    }
    
}