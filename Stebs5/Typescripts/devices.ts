module Stebs {
    export interface Device {
        deviceName: string;
        deviceTextData: string[];
        deviceNumberData: number[];

        init(): void;
        serverToDevice(textData: string[], numberData: number[]): void;
        deviceToServer(interrupt: boolean): void;
    }

    export abstract class BasicDevice implements Device {
        public deviceName: string = "BasicDevice";
        public deviceTextData: string[];
        public deviceNumberData: number[];

        /**
         * Initalize the basic device.
         */
        public init() {
            Stebs.devices[this.deviceName] = this;
        }
        /**
         * Set received data from server.
         * @param textData all text data.
         * @param numberData all number data.
         */
        public serverToDevice(textData: string[], numberData: number[]) {
            this.deviceTextData = textData;
            this.deviceNumberData = numberData;
        }
        /**
         * Send server the device data.
         * @param interrupt send with interrupt then send true.
         */
        public deviceToServer(interrupt: boolean) {
            Stebs.serverHub.deviceToServer(this.deviceName, this.deviceTextData, this.deviceNumberData, interrupt);
        }
    }

    export class InterruptDevice extends BasicDevice {
        public init() {
            this.deviceName = "InterruptDevice";
            super.init();
            $('#interruptButton').click(() => this.clickedInterruptTest());
            $('#inputTest').click(() => this.clickedInputTest());
            $('#outputTest').text('initialized');
        }
        public clickedInterruptTest = () => {
            this.deviceToServer(true);
        }
        public clickedInputTest = () => {
            this.deviceToServer(false);
        }
        public serverToDevice(textData: string[], numberData: number[]) {
            super.serverToDevice(textData, numberData);
            console.log("setData from server " + textData[0]);
            if (textData.length == 1) {
                $('#outputTest').text(textData[0]);
            } else if (numberData.length == 1) {
                $('#outputTest').text(numberData[0]);
            }
        }
    }

}