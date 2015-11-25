module Stebs {

    export class FileManagement {
        public rootNode = new FileNode(0,'root', null, true, []);
        public actualNode: FileNode = this.rootNode;
        public addMode: boolean = false;

        constructor() {
            var inner = new FileNode(1, 'inner', this.rootNode, true, []);
            this.rootNode.appendChild(inner);
            var inner2 = new FileNode(2, 'inner2', inner, false, []);
            inner.appendChild(inner2);
            
            this.actualNode = this.rootNode;
        }

        public init() {
            var fileManagement = this;
            this.setAndShowActualNode(this.actualNode);
            $('.fileSystem').hide();

            $('#open').click(function () {
                $('.fileSystem').show();
            });

            $('#new').click(function () {
                $('.fileSystem').show();
            });

            $('#save').click(function () {
                console.log("save called");
            });
            $('#addFile').click(function () {
                if (!fileManagement.addMode) {
                    var actualNode = Stebs.fileManagement.actualNode;
                    var newNode = new FileNode(-1, 'new File', actualNode, false, []);
                    actualNode.appendChild(newNode);
                    //setEditable
                    fileManagement.setAndShowActualNode(actualNode);
                    newNode.setEditable();
                    fileManagement.addMode = true;
                }
            });
            $('#addFolder').click(function () {
                if (!fileManagement.addMode) {
                    var actualNode = Stebs.fileManagement.actualNode;
                    var newNode = new FileNode(-1, 'new Folder', actualNode, true, []);
                    actualNode.appendChild(newNode);
                    //setEditable
                    fileManagement.setAndShowActualNode(actualNode);
                    newNode.setEditable();
                    fileManagement.addMode = true;
                }
            });
        }
        public setAndShowActualNode(fileNode: FileNode): void {
            if (fileNode.isFolder()) {
                this.actualNode = fileNode;
                this.showFileManagement(fileNode);
                this.showActualPath();
            } else {
                //hideView
                $('.fileSystem').hide();
                console.log('openfile');
            }
        }
        public showFileManagement(fileNode: FileNode): void {
            $('#files').empty();
            var childrenAsJQuery: JQuery[] = fileNode.asFolderHTML();
            for (var i = 0; i < childrenAsJQuery.length; i++) {
                $('#files').append(childrenAsJQuery[i]);
            }
        }
        public showActualPath(): void {
            var links: JQuery[] = [];
            $('#folderPath').empty()
            var actualNode = this.actualNode;
            var parentNode = actualNode.getParent();
            while (parentNode != null) {
                var node = actualNode;
                var link = $('<a>')
                    .prop('href', '#')
                    .text(node.getNodename())
                    .click(function () {
                        Stebs.fileManagement.setAndShowActualNode(node);
                    });
                links.push(link);
                actualNode = parentNode;
                parentNode = parentNode.getParent();
            }
            $('#folderPath').append('/');
            var rootNode = this.rootNode;
            var rootLink = $('<a>')
                .prop('href', '#')
                .text(rootNode.getNodename())
                .click(function () {
                    Stebs.fileManagement.setAndShowActualNode(rootNode);
                });
            $('#folderPath').append(rootLink);
            $('#folderPath').append('/');
            for (var i = links.length - 1; i >= 0; i--) {
                $('#folderPath').append(links[i]);
                $('#folderPath').append('/');
            }
        }

        public exitAddMode() {
            if (this.addMode) {
                var toDelete = this.actualNode.getById(-1);
                console.log(toDelete);
                if (toDelete != null) {
                    toDelete.deleteFile();
                }
                this.addMode = false;
            }
        }

    }

    export class FileNode {
        private id: number;
        private nodeName: string = '';
        private parent: FileNode;
        private fileIsFolder: boolean;
        private childs: FileNode[] = [];

        constructor(id: number, nodeName: string, parent: FileNode, isFolder: boolean, childs: FileNode[]) {
            this.id = id;
            this.nodeName = nodeName;
            this.parent = parent;
            this.fileIsFolder = isFolder;
            this.childs = childs;
            if (childs.length > 0) {
                this.fileIsFolder = true;
            }
        }

        public getId(): number {
            return this.id;
        }

        public getNodename(): string {
            return this.nodeName;
        }

        public getChilds(): FileNode[] {
            return this.childs;
        }

        public getById(id: number): FileNode {
            if (this.id == id) {
                return this;
            } else {
                for (var i = 0; i < this.childs.length; i++) {
                    var child = this.childs[i].getById(id);
                    if (child != null) {
                        return child;
                    }
                }
                return null;
            }
        }

        public getParent(): FileNode {
            return this.parent;
        }

        public isFolder(): boolean {
            return this.fileIsFolder;
        }

        public appendChild(newChild: FileNode): void {
            this.childs.push(newChild);
        }

        public asFolderHTML(): JQuery[] {
            var nodes: JQuery[] = [];

            for (var i = 0; i < this.childs.length; i++) {
                nodes.push(this.childs[i].asHTML());
            }
            return nodes;
        }

        public setEditable(): void {
            var node = this;
            var editableText = $('<input>')
                .prop('type', 'text')
                .val($('#file-' + this.id + ' a.openLink').text());
            $('#file-' + this.id + ' a.openLink').replaceWith(editableText);
            var okLink = $('<a>')
                
            $('#file-' + this.id + ' a.editElement')
                .removeClass('editElement')
                .addClass('okElement')
                .prop('href', '#')
                .click(function () {
                    node.setText();
                });

            $('#file-' + this.id + ' a.removeElement')
                .removeClass('removeElement')
                .addClass('cancelElement')
                .prop('href', '#')
                .click(function () {
                    node.cancelEditMode();
                });
        }

        public setText(): void {
            var node = this;
            var newName = $('#file-' + this.id + ' input').val();
            this.nodeName = newName;
            //sendToServer
            var textSpan = $('<span/>')
                .addClass('text')
                .text(this.nodeName);
            var openLink = $('<a>')
                .prop('href', '#')
                .addClass('openLink')
                .append(textSpan)
                .click(function () {
                    Stebs.fileManagement.setAndShowActualNode(node);
                });
            $('#file-' + this.id + ' input').replaceWith(openLink);

            $('#file-' + this.id + ' a.okElement')
                .removeClass('okElement')
                .addClass('editElement')
                .prop('href', '#')
                .click(function () {
                    node.setEditable();
                });

            $('#file-' + this.id + ' a.cancelElement')
                .removeClass('cancelElement')
                .addClass('removeElement')
                .prop('href', '#')
                .click(function () {
                    node.deleteFile();
                });
            Stebs.fileManagement.addMode = false;
        }

        public cancelEditMode(): void {
            var node = this;
            $('#file-' + this.id + ' input').val(this.nodeName);
            this.setText();

            $('#file-' + this.id + ' a.cancelElement')
                .removeClass('cancelElement')
                .addClass('removeElement')
                .prop('href', '#')
                .click(function () {
                    node.deleteFile();
                });
        }

        public deleteFile(): void {
            var parent = this.parent;
            var index = parent.childs.indexOf(this);
            if (index != -1) {
                this.parent.childs.splice(index, 1);
            }
            if (parent == null) {
                parent = Stebs.fileManagement.rootNode;
            }
            Stebs.fileManagement.setAndShowActualNode(parent);
            Stebs.fileManagement.exitAddMode();
        }

        public asHTML(): JQuery {
            var node = this;
            var nodeJQuery = $('<div>')
                .addClass('file-node')
                .prop('id', 'file-' + this.id);
            var imgSpan = $('<span/>')
                .addClass('img');
            if (this.isFolder()) {
                imgSpan.addClass('folder');
            }
            nodeJQuery.append(imgSpan);

            var textSpan = $('<span/>')
                .addClass('text')
                .text(this.nodeName);
            var openLink = $('<a>')
                .addClass('openLink')
                .prop('href', '#')
                .append(textSpan)
                .click(function () {
                    Stebs.fileManagement.setAndShowActualNode(node);
                });
            nodeJQuery.append(openLink);
            var editJQuery = $('<a>')
                .addClass('img')
                .addClass('editElement')
                .prop('href', '#')
                .click(function () {
                    node.setEditable();
                });
            nodeJQuery.append(editJQuery);
            var deleteJQuery = $('<a>')
                .addClass('img')
                .addClass('removeElement')
                .prop('href', '#')
                .click(function () {
                    node.deleteFile();
                });
            nodeJQuery.append(deleteJQuery);
            return nodeJQuery;
        }
    };

}