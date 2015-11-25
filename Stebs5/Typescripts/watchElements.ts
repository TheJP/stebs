module Stebs {

    export class WatchElement {
        private name: string;
        public value: number;

        constructor(name: string, value: number) {
            this.name = name;
            this.value = value;
        }

        public getName(): string {
            return name;
        }

        public setName(newName: string) {
            this.name = newName;
        }

        public getValue(): number {
            return this.value;
        }

        public setValue(newValue: number): void {
            this.value = newValue;
        }

        public asHtml(): JQuery {

        }

    }
    
}