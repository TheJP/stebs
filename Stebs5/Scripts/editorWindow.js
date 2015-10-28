var Stebs;
(function (Stebs) {
    Stebs.coloredItems = {
        test: 10
    };
    var EditorWindow = (function () {
        function EditorWindow() {
            $('#editorWindow').contents().prop('designMode', 'on');
            this.editorDoc = $('#editorWindow')[0];
            this.inkText();
        }
        EditorWindow.prototype.inkText = function () {
            var inkedText = "";
            var span = $(document.createElement('span'));
            $('#editorWindow').contents().find("html").each(function () {
                var foo = $(this).html();
                console.log(foo);
            });
            this.formatText(this.editorDoc, 'insertBrOnReturn', false);
            /*
            var isComment: boolean = false;
            var isMemoryPos: boolean = false;
            for (var i = 0; i < textInEditor.length; i++) {
                console.log(textInEditor[i]);
                if (textInEditor[i].indexOf(";") != -1) {
                    isComment = true;
                }
                if (textInEditor[i].indexOf(":") != -1) {
                    
                }
                if (textInEditor[i].indexOf("[") != -1) {
                    isMemoryPos = true;
                }
                if (textInEditor[i].indexOf("]") != -1) {
                    isMemoryPos = false;
                }
                if (textInEditor[i].indexOf("\n") != -1) {
                    isComment = false;
                    isMemoryPos = false;
                    inkedText += span;
                    $('#editorWindow').contents().find("html").text(inkedText);
                }
                if (isComment) {
                    span.addClass("error").text(span.text() + textInEditor[i]);
                }*/
        };
        EditorWindow.prototype.formatText = function (iframe, command, option) {
            iframe.contentWindow.focus();
            try {
                iframe.contentWindow.document.execCommand(command, false, option);
            }
            catch (e) {
                console.log(e);
            }
            iframe.contentWindow.focus();
        };
        return EditorWindow;
    })();
    Stebs.EditorWindow = EditorWindow;
})(Stebs || (Stebs = {}));
