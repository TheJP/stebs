var stebs = {};

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
stebs.hello = function hello(word){
    stebs.ctx.fillStyle = 'green';
    stebs.ctx.font = '100pt Helvetica';
    stebs.ctx.textAlign = 'center';
    stebs.ctx.fillText(word, stebs.canvas.width / 2, stebs.canvas.height / 2);
};

$(document).ready(function (){

    stebs.ctx = $('#canvas')[0].getContext('2d');
    stebs.canvas = stebs.ctx.canvas;
    stebs.normalizeCanvas();
    var hub = $.connection.stebsHub;

    hub.client.hello = stebs.hello;

    $.connection.hub.start().done(function () {
        hub.server.hello('you');
    });
});