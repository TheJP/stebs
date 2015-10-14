var stebs = {};

stebs.visible = {};
stebs.visible.devices = false;
stebs.visible.architecture = false;

/**
 * Stores a global reference of the canvas and sets the global style.
 */
stebs.setupCanvas = function setupCanvas() {
    stebs.ctx = $('#canvas')[0].getContext('2d');
    stebs.canvas = stebs.ctx.canvas;
    stebs.normalizeCanvas();

    stebs.ctx.font = '20pt Helvetica';
    stebs.ctx.textAlign = 'center';
}

/**
 * Resize canvas to real size (otherwise the content gets stretched).
 */
stebs.normalizeCanvas = function normalizeCanvas(ctx) {
    var width = parseInt($('#canvas').css('width'), 10);
    var height = parseInt($('#canvas').css('height'), 10);
    if (stebs.canvas.width != width || stebs.canvas.height != height) {
        stebs.canvas.width = width;
        stebs.canvas.height = height;
    }
};

/**
 * Hello callback function.
 */
stebs.hello = function hello(word) {
    stebs.ctx.fillStyle = 'green';
    stebs.ctx.font = '100pt Helvetica';
    stebs.ctx.fillText(word, stebs.canvas.width / 2, stebs.canvas.height / 2);
};

/** Opens/Closes the devices sidebar. */
stebs.toggleDevices = function toggleDevices() {
    var animation = { left: (stebs.visible.devices ? '-=400px' : '+=400px') };
    $('#devices, #architecture').animate(animation);
    animation.width = (stebs.visible.devices ? '+=400px' : '-=400px');
    $('#codingView').animate(animation);
    stebs.visible.devices = !stebs.visible.devices;
};

/** Opens/Closes the architecture sidebar. */
stebs.toggleArchitecture = function toggleArchitecture() {
    var animation = { left: (stebs.visible.architecture ? '-=400px' : '+=400px') };
    $('#architecture').animate(animation);
    animation.width = (stebs.visible.architecture ? '+=400px' : '-=400px');
    $('#codingView').animate(animation);
    stebs.visible.architecture = !stebs.visible.architecture;
};

$(document).ready(function (){

    $('#editorWindow').contents().prop('designMode', 'on');
    stebs.setupCanvas();

    var hub = $.connection.stebsHub;
    hub.client.hello = stebs.hello;

    $.connection.hub.start().done(function () {
        hub.server.hello('you');
    });

    $('#openDevices').click(stebs.toggleDevices);
    $('#openArchitecture').click(stebs.toggleArchitecture);
    $(window).resize(function windowResize() {
        var minus = 50 + 400 * ((stebs.visible.architecture ? 1 : 0) + (stebs.visible.devices ? 1 : 0));
        $('#codingView').animate({ width: window.innerWidth -  minus}, 0);
    });

});