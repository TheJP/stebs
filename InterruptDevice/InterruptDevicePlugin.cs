using PluginApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessorSimulation.Device;

namespace InterruptDevice
{
    public class InterruptDevicePlugin : IDevicePlugin
    {
        public string Name => "Interrupt Device";

        public string PluginId => "InterruptDevice";

        public IDevice CreateDevice() => new InterruptDevice();

        public string DeviceTemplate(byte slot) =>

$@"<style>
    .interrupt-device {{
        padding-top: 10px;
        padding-bottom: 10px;
    }}
    .interrupt-device input[type=range] {{
        width: 150px;
    }}
</style>
<div class=""interrupt-device"">
    <button id=""interrupt-{slot}"">Single Interrupt</button><br />
    <label><input type=""checkbox"" id=""interrupt-enable-{slot}"" /> Periodic Interrupts</label>
    <input type=""range"" id=""interrupt-time-{slot}"" min=""0"" max=""100"" value=""34"" />
</div>
<script>
    $('#interrupt-{slot}').click(function(){{
        Stebs.updateDevice({slot}, 'InterruptOnce');
    }});
    $('#interrupt-time-{slot}').change(function(){{
        var time = Stebs.ui.logarithmicValue(parseInt($('#interrupt-time-{slot}').val()), 30000, 10);
        Stebs.updateDevice({slot}, '{{ ""Command"": ""ChangeIntervalCommand"", ""NewInterval"": ' + parseInt(time) + ' }}');
    }});
    $('#interrupt-enable-{slot}').change(function(){{
        Stebs.updateDevice({slot}, $('#interrupt-enable-{slot}').prop('checked') ? 'ActivateInterrupts' : 'DisableInterrupts');
    }});
</script>";

    }
}
