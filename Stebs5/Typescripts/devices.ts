module Stebs {
    export interface Device {
        deviceName: string;
        deviceTextData: string[];
        deviceNumberData: number[];

        init(): void;
        serverToDevice(textData: string[], numberData: number[]): void;
        deviceToServer(): void;
    }

    export abstract class BasicDevice implements Device {
        public deviceName: string = "BasicDevice";
        public deviceTextData: string[];
        public deviceNumberData: number[];

        public init() {
            Stebs.devices[this.deviceName] = this;
        }
        public serverToDevice(textData: string[], numberData: number[]) {
            this.deviceTextData = textData;
            this.deviceNumberData = numberData;
        }
        public deviceToServer() {
            Stebs.serverHub.deviceToServer(this.deviceName, this.deviceTextData, this.deviceNumberData);
        }
    }

    export class InterruptDevice extends BasicDevice {
        public init() {
            this.deviceName = "InterruptDevice";
            super.init();
            $('#interruptButton').click(function () {

            });
            $('#inputTest').click(() => this.clicked());
            $('#outputTest').text('initialized');
        }

        public clicked = () => {
            this.deviceToServer();
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