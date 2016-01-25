module Stebs {

    /**
     * Manager which is responsible for the client side handling of devices.
     */
    export var deviceManager = {

        private deviceTypes: <{ [id: string]: DeviceType }>{},

        /**
         * Initializes the manager.
         */
        init(): void {
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

    };

    export class DeviceType {
        public Name: string;
        public Id: string;
    };

}