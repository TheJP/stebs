module Stebs {
    export var coloredItems = {
        test: 10
    };

    export var colorsWords = {
        comments: 'green',
        architecture: false,
        ram: false,
        output: false
    };
    
    export class EditorWindow {
        private editorDoc: HTMLFrameElement;
        private editableDiv: HTMLElement;

        constructor() {
            $('#editorWindow').contents().prop('designMode', 'on');
            this.editorDoc = <HTMLFrameElement>$('#editorWindow')[0];
            $('#editableDiv').attr('contenteditable', 'true');
            //this.inkText();
        }
        
        public inkText(): void {
            var caretPos = this.getCaretCharOffsetInDiv($('#editableDiv').get(0));
            console.log(caretPos);

            $('#editableDiv').each(function (i, elem) {
                var regex = /<br\s*[\/]?>/gi;
                var newHtml = $(this).html().replace(regex, "\n");
                console.log(newHtml);
                var lines = newHtml.split("\n");
                for (var i = 0; i < lines.length; i++) {
                    var text = lines[i].split(' ');

                    for (var j = 0; j < text.length; j++) {
                        if (text[j].length > 0) {
                            var subStart = text[j].indexOf(">");
                            if (subStart < 0) {
                                subStart = 0;
                            } else {
                                subStart += 1;
                            }
                            var subEnd = text[j].lastIndexOf("<");
                            if (subEnd < 0) {
                                subEnd = text[j].length;
                            }
                            text[j] = text[j].substring(subStart, subEnd);
                            if (text[j].length > 0) {
                                text[j] = '<span class="word">' + text[j] + '</span>';
                            }
                        }
                    }
                    lines[i] = text.join(' ');
                }

                $(this).html(lines.join('<br>'));
            });
            /*
                if (text.indexOf(":") != -1) {
                if (text.indexOf("[") != -1) {
                if (text.indexOf("]") != -1) {
            */
        }

        public getCaretCharOffsetInDiv(element: HTMLElement): number {
            var caretOffset = 0;
            if (typeof window.getSelection != "undefined") {
                var range = window.getSelection().getRangeAt(0);
                var preCaretRange = range.cloneRange();
                preCaretRange.selectNodeContents(element);
                preCaretRange.setEnd(range.endContainer, range.endOffset);
                caretOffset = preCaretRange.toString().length;
            }
            return caretOffset;
        }

        public formatText(iframe: HTMLFrameElement, command: string, option: string | boolean): void {
            iframe.contentWindow.focus();
            try {
                iframe.contentWindow.document.execCommand(command, false, option);
            } catch (e) { console.log(e) }
            iframe.contentWindow.focus();
        }

        
    }
}