module Stebs {

    export var registerControl = {
        registers: <{ [register: string]: Register } >{},
        defaultRegisters: <string[]>['AL', 'BL', 'CL', 'DL', 'IP', 'SP'],
        propagateToRam: <string[]>['IP', 'SP'],

        init(): void {
            $.connection.stebsHub.server.loadRegisters();
            $('#newWatchesButton').click(function () {
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
                                Stebs.registerControl.registerAddWatch(name);
                                $('#addWatches').hide();
                            });
                    } ();
                    $('#elementList').append(link);
                }
                var offset = $('#newWatchesButton').offset();
                $('#addWatches').css('left', offset.left).css('bottom', Stebs.heights.runAndDebug);
                $('#addWatches').show();
            });

            $('#closeWatches').click(function () {
                $('#addWatches').hide();
            });
        },

        addAll(registersByNames: string[]) {
            for (var i = 0; i < registersByNames.length; i++) {
                var newRegister = new Register(registersByNames[i]);
                this.registers[registersByNames[i]] = newRegister;
                if (Stebs.registerControl.defaultRegisters.indexOf(registersByNames[i]) != -1) {
                    newRegister.addWatchElement();
                }
            }
        },

        updateRegister(name: string, value: number) {
            if (Stebs.registerControl.registers[name] != null) {
                Stebs.registerControl.registers[name].updateValue(value);
                if (name == 'IP') {
                    Stebs.ramContent.setInstructionPointer(value);
                }
                if (name == 'SP') {
                    Stebs.ramContent.setStackPointer(value);
                }
            }
        },

        registerAddWatch(name: string) {
            if (Stebs.registerControl.registers[name] != null) {
                Stebs.registerControl.registers[name].addWatchElement();
            }
        },

        registersWithoutWatches(): string[] {
            var registerNames: string[] = [];
            for (var name in registerControl.registers) {
                var register = registerControl.registers[name];
                if (!register.hasWatchElemet()) {
                    registerNames.push(name);
                }
            }
            return registerNames;
        },

        addWatch(name: string) {
            if (Stebs.registerControl.registers[name] != null) {
                registerControl.registers[name].addWatchElement();
            }
        }

    };
    
    export var watchControl = {
        show(watchElement: WatchElement): void {
            $('#watcher-elements').append(watchElement.asJQuery());
        },

        remove(watchElement: WatchElement): void {
            $('#watch-' + watchElement.getRegister().getName()).remove();
            watchElement.getRegister().removeWatchElement();
        },

        updateElement(watchElement: WatchElement): void {
            $('#watch-' + watchElement.getRegister().getName() + ' .watch-element-value')
                .text(watchElement.getValueFormated());
        },

        setToBinayORHex(watchElement: WatchElement): void {
            watchElement.showAsBinaryOrHex();
            $('#watch-' + watchElement.getRegister().getName() + ' .watch-element-value')
                .text(watchElement.getValueFormated())
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
                Stebs.watchControl.updateElement(this.watchElement);
            }
        }

        public addWatchElement() {
            this.watchElement = new WatchElement(this);
            Stebs.watchControl.show(this.watchElement);
        }

        public removeWatchElement() {
            //Stebs.watchControl.remove(this.watchElement);
            this.watchElement = null;
        }
    };

    export class WatchElement {
        private register: Register;
        private showBinary: boolean = false;

        constructor(register: Register) {
            this.register = register;
        }

        public getRegister(): Register {
            return this.register;
        }

        public showAsBinaryOrHex(): void {
            this.showBinary = !this.showBinary;
        }

        public getValueFormated(): string {
            if (this.showBinary) {
                var asBinay = Stebs.utility.addLeadingZeros(this.register.getValue(), 2, 8);
                var asBinaryValue = asBinay.slice(0, 4);
                asBinaryValue += '\'';
                asBinaryValue += asBinay.slice(4, 8);
                return asBinaryValue;
            }
            return Stebs.utility.addLeadingZeros(this.register.getValue(), 16, 2);
        }

        public asJQuery(): JQuery {
            var myName = this.register.getName();
            var mySelf = this;
            var nameP = $('<p>')
                .text(myName);
            var linkP = $('<a>')
                .prop('href', '#')
                .addClass('watch-element-value')
                .text(this.getValueFormated())
                .click(function () {
                    Stebs.watchControl.setToBinayORHex(mySelf);
                });
            var closeButton = $('<button>')
                .text('x')
                .click(function () {
                    Stebs.watchControl.remove(mySelf);
                });
            var element = $('<div>')
                .prop('id', 'watch-' + myName)
                .addClass('watcher')
                .append(closeButton)
                .append(nameP)
                .append(linkP);
            return element;
        }

    }
    
}