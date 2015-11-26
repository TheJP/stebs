module Stebs {
    
    export var watchControl = {
        availableElements: <String[]>[],
        watchElements: <WatchElement[]>[],

        setAvailableElements(elements: string[]): void {
            Stebs.watchControl.availableElements = elements;
        },

        add(name: string, value: number): void {
            if (Stebs.watchControl.availableElements.indexOf(name) != -1) {
                var newElement = new WatchElement(name, value);
                Stebs.watchControl.watchElements.push(newElement);
                $('.watcher').prepend(newElement.asJQuery());
            } else {
                console.log('element does not exist');
            }
        },

        addEmpty(name: string): void {
            Stebs.watchControl.add(name, 0);
        },

        addAll(names: string[]): void {
            for (var i = 0; i < names.length; i++) {
                Stebs.watchControl.addEmpty(names[i]);
            }
        },

        removeByName(name: string): void {
            var watchElements = Stebs.watchControl.watchElements;
            var infoWatchElement = Stebs.watchControl.getByName(name);
            if (infoWatchElement != null) {
                watchElements.splice(infoWatchElement.posInArray);
                $('#watch-' + infoWatchElement.watchElement.getName()).remove();
            }
        },

        getByName(name: string): { posInArray: number, watchElement: WatchElement } {
            var watchElements = Stebs.watchControl.watchElements;
            for (var i = 0; i < watchElements.length; i++) {
                if (watchElements[i].getName() == name) {
                    return { posInArray: i, watchElement:watchElements[i] };
                }
            }
            return null;
        },

        updateElement(name: string, value: number): boolean {
            var watchElements = Stebs.watchControl.watchElements;
            var infoWatchElement = Stebs.watchControl.getByName(name);
            if (infoWatchElement != null) {
                watchElements[infoWatchElement.posInArray].updateValue(value);
                $('#watch-' + infoWatchElement.watchElement.getName() + ' .watch-element-value').text(value);
                return true;
            }
            return false;
        }
    };

    export class WatchElement {
        private name: string;
        private value: number;

        constructor(name: string, value: number) {
            this.name = name;
            this.value = value;
        }

        public getName(): string {
            return name;
        }

        public getValue(): number {
            return this.value;
        }

        public updateValue(newValue: number): void {
            this.value = newValue;
        }

        public asJQuery(): JQuery {
            var myName = this.name;
            var nameP = $('<p>')
                .text(this.name);
            var valueP = $('<p>')
                .addClass('watch-element-value')
                .text(this.value);
            var closeButton = $('<button>')
                .click(function () {
                    Stebs.watchControl.removeByName(myName);
                });
            var element = $('<div>')
                .prop('id', 'watch-' + this.name)
                .addClass('watchElement')
                .append(closeButton)
                .append(nameP)
                .append(valueP);
            return element;
        }

    }
    
}