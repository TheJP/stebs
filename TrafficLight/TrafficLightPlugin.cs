using PluginApi;
using ProcessorSimulation.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficLightPlugin : IDevicePlugin
    {

        public string Name => "Traffic Light";

        public string PluginId => "TrafficLight";

        public IDevice CreateDevice() => new TrafficLightDevice();

        /// <remarks>Since html5, style tags are allowed in the body tag.</remarks>
        public string DeviceTemplate(byte slot) =>

$@"<style>
    #lights-{slot} span {{
        display: block;
        width: 30px;
        height: 30px;
        border: 1px solid white;
        border-radius: 50%;
        margin-bottom: 5px;
    }}
    #lights-{slot} span:nth-child(1) {{
        background-color: red;
    }}
    #lights-{slot} span:nth-child(2) {{
        background-color: yellow;
    }}
    #lights-{slot} span:nth-child(3) {{
        background-color: lime;
        display: inline-block;
    }}
    #lights-{slot} span:nth-child(4) {{
        background-color: yellow;
        display: inline-block;
    }}
    .lights-right, .lights-left {{
        float: left;
        margin-top: 10px;
        margin-right: 10px;
    }}
    .traffic-data {{
        font-size: 25px;
        left: 20px;
        position: relative;
        top: 40px;
    }}
</style>
<div id=""lights-{slot}"">
    <div class=""lights-left"">
        <span></span>
        <span></span>
        <span></span>
        <span></span>
    </div>
    <div class=""lights-right"">
        <span></span>
        <span></span>
        <span></span>
        <span></span>
    </div>
</div>
<div class=""traffic-data"">
    <span id=""traffic-data-{slot}"">1111'1111</span>
</div>
<script>
    Stebs.registerDevice({slot}, function(data){{
        var binary = Stebs.convertNumber(data.Data, 2, 8)
        $('#traffic-data-{slot}').text(binary.slice(0, 4) + '\'' + binary.slice(4, 8));
        var byte = data.Data;
        var lights = $('#lights-{slot} span');
        var bit = 128;
        for(var i = 0; i < 8; ++i, bit >>= 1){{
            if((byte & bit) != 0){{ $(lights[i]).removeAttr('style'); }}
            else {{ $(lights[i]).css('border', '1px solid black').css('background-color', 'white'); }}
        }}
    }});
</script>";

    }
}
