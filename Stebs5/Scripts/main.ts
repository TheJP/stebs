module Stebs {
    export var visible = {
        devices: false,
        architecture: false
    };

    var ctx: any;
    var canvas: any;

    export var ui = {

        /**
         * Stores a global reference of the canvas and sets the global style.
         */
        setupCanvas(): void {
            //ctx = $('#canvas')[0].getContext('2d');
            ctx = {};
            canvas = ctx.canvas || {};
            this.normalizeCanvas();

            ctx.font = '20pt Helvetica';
            ctx.textAlign = 'center';
        },

        /**
         * Resize canvas to real size (otherwise the content gets stretched).
         */
        normalizeCanvas(): void {
            var width = parseInt($('#canvas').css('width'), 10);
            var height = parseInt($('#canvas').css('height'), 10);
            if (canvas.width != width || canvas.height != height) {
                canvas.width = width;
                canvas.height = height;
            }
        },

        /**
         * Hello callback function.
         */
        hello(word: string): void {
            ctx.fillStyle = 'green';
            ctx.font = '100pt Helvetica';
            ctx.fillText(word, canvas.width / 2, canvas.height / 2);
        },

        /**
         * Opens/Closes the devices sidebar.
         */
        toggleDevices(): void {
            var animation = { left: (visible.devices ? '-=400px' : '+=400px') };
            $('#devices, #architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.devices ? '+=400px' : '-=400px') };
            $('#codingView').animate(animation2);
            visible.devices = !visible.devices;
        },

        /**
         * Opens/Closes the architecture sidebar.
         */
        toggleArchitecture(): void {
            var animation = { left: (visible.architecture ? '-=400px' : '+=400px') };
            $('#architecture').animate(animation);
            var animation2 = { left: animation.left, width: (visible.architecture ? '+=400px' : '-=400px') };
            $('#codingView').animate(animation2);
            visible.architecture = !visible.architecture;
        }
    };
}

$(document).ready(function (){

    $('#editorWindow').contents().prop('designMode', 'on');
    Stebs.ui.setupCanvas();

    /*var hub = $.connection.stebsHub;
    hub.client.hello = stebs.hello;

    $.connection.hub.start().done(function () {
        hub.server.hello('you');
    });*/

    $('#openDevices').click(Stebs.ui.toggleDevices);
    $('#openArchitecture').click(Stebs.ui.toggleArchitecture);
    $(window).resize(function windowResize() {
        var minus = 50 + (Stebs.visible.architecture ? 400 : 0) + (Stebs.visible.devices ? 400 : 0);
        $('#codingView').css('width', (window.innerWidth -  minus) + 'px');
    });

});