module Stebs {

    export function registerDevice(slot: number, callback: (data: any) => void) {
        deviceManager.updateCallbacks[slot] = callback;
    };

    export function updateDevice(slot: number, update: any) {
        serverHub.updateDevice(slot, update);
    };

    /**
     * Manager which is responsible for the client side handling of devices.
     */
    export var deviceManager = {

        private deviceTypes: <{ [id: string]: DeviceType }>{},
        private updateCallbacks: <{ [slot: number]: (data: any) => void }>{},

        /**
         * Initializes the manager.
         */
        init(): void {
            $('#addDeviceForm').submit(() => {
                var deviceType = $('#deviceSelect').val();
                var slot = parseInt($('#deviceSlot').val());
                serverHub.addDevice(deviceType, slot)
                   .then(result => { if (result.Success) { deviceManager.addDevice(deviceManager.deviceTypes[deviceType], result) } });
                return false;
            });
        },

        /**
         * Sets the device types.
         */
        setDeviceTypes(types: { [id: string]: DeviceType }): void {
            deviceManager.deviceTypes = types;
            //Add options to the device adding dialog
            var select = $('#deviceSelect');
            select.empty()
            for (var id in deviceManager.deviceTypes) {
                var deviceType = deviceManager.deviceTypes[id];
                select.append($('<option />').text(deviceType.Name).val(deviceType.Id));
            }
        },

        /**
         * Add needed gui elements to add the new device.
         * @param deviceType
         * @param slot
         */
        addDevice(deviceType: DeviceType, device: AddDeviceViewModel): void {
            $('#deviceElements').append(
                $('<div />')
                    .attr('id', 'device-' + device.Slot)
                    .addClass('device')
                    .html(device.Template)
                    .prepend($('<p />').text('[' + device.Slot + '] ' + deviceType.Name))
            );
        },

        updateView(slot: number, update: any) {
            if (deviceManager.updateCallbacks[slot]) { deviceManager.updateCallbacks[slot](update); }
        }

    };

    export class DeviceType {
        public Name: string;
        public Id: string;
    };

}
