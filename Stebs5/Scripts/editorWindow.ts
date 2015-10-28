

module Stebs {
    export var coloredItems = {
        test: 10
    };
    
    export class EditorWindow {
        private editorDoc: HTMLFrameElement;

        constructor() {
            $('#editorWindow').contents().prop('designMode', 'on');
            this.editorDoc = <HTMLFrameElement>$('#editorWindow')[0];
            this.inkText();
        }

        public inkText(): void {
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
            }

            public formatText(iframe: HTMLFrameElement, command: string, option): void {
                iframe.contentWindow.focus();
                try {
                    iframe.contentWindow.document.execCommand(command, false, option);
                } catch (e) { console.log(e) }
                iframe.contentWindow.focus();
            }

        }
    
}