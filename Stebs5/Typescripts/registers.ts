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
            var registerNames = registerControl.registersWithoutWatches();
            $('#elementList').empty();
            for (var i = 0; i < registerNames.length; i++) {
                var link = function () {
                    var name = registerNames[i];
                    return $('<a>')
                        .prop('href', '#')
                        .addClass('registerLink')
                        .text(name)
                        .click(function () {
                            registerControl.addWatch(name);
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
        addAll(registersByNames: string[]) {
            for (var i = 0; i < registersByNames.length; i++) {
                var newRegister = new Register(registersByNames[i]);
                this.registers[registersByNames[i]] = newRegister;
                if (registerControl.defaultRegisters.indexOf(registersByNames[i]) != -1) {
                    newRegister.addWatchElement();
                }
            }
        },

        updateRegister(name: string, value: number) {
            registerControl.registers[name].updateValue(value);
            if (name == 'IP') {
                ramContent.setInstructionPointer(value);
            } else if (name == 'SP') {
                ramContent.setStackPointer(value);
            }
        },

        addWatch(name: string) {
            registerControl.registers[name].addWatchElement();
        },

        /**
         * Returns an array, which contains all register types that don't have a watch yet.
         */
        registersWithoutWatches(): string[] {
            var registerNames: string[] = [];
            for (var name in registerControl.registers) {
                var register = registerControl.registers[name];
                if (!register.hasWatchElemet()) { registerNames.push(name); }
            }
            return registerNames;
        }

    };
    
    export var watchControl = {

        resetHighlightedElements(): void {
            $('.watcher').removeClass('changed');
        }

    };

    export class Register {

        private name: string;
        private value: number;
        private watchElement: WatchElement;

        constructor(name: string) {
            this.name = name;
            this.value = 0;
        }

        public getName(): string {
            return this.name;
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
                this.watchElement.update();
            }
        }

        public addWatchElement() {
            this.watchElement = new WatchElement(this);
            this.watchElement.show();
        }

        public removeWatchElement() {
            this.watchElement = null;
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
            return this.getRegister().getName();
        }

        /** Adds a new watch of this register type to the watcher elements. */
        show(): void {
            $('#watcher-elements').append(this.asJQuery());
        }

        /** Removes this watch from the watcher elements. */
        remove(): void {
            $('#watch-' + this.getType()).remove();
            this.getRegister().removeWatchElement();
        }

        /** Updates the view of this watcher element. */
        update(): void {
            $('#watch-' + this.getType() + ' .watch-element-value').text(this.getValueFormated());
            $('#watch-' + this.getType()).addClass('changed');
        }

        /** Toggles element between binary and hex representation. */
        toggleBinaryOrHex(): void {
            this.showBinary = !this.showBinary;
            $('#watch-' + this.getType() + ' .watch-element-value').text(this.getValueFormated())
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
            var name = $('<p>').text(this.getType());
            var link = $('<a>')
                .prop('href', '#')
                .addClass('watch-element-value')
                .text(this.getValueFormated())
                .click(() => this.toggleBinaryOrHex());
            var closeButton = $('<button>')
                .text('x')
                .click(() => this.remove());
            var element = $('<div>')
                .prop('id', 'watch-' + this.getType())
                .addClass('watcher')
                .append(closeButton)
                .append(name)
                .append(link);
            return element;
        }

    }
    
}