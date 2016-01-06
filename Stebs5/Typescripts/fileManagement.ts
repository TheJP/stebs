module Stebs {

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

        public setId(id: number): void {
            $('#file-' + this.id).prop('id', id);
            this.id = id;
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

        public setFilenameEditable(): void {
            var me = this;
            var editableText = $('<input>')
                .prop('type', 'text')
                .val($('#file-' + this.id + ' a.openLink').text());
            $('#file-' + this.id + ' a.openLink').replaceWith(editableText);
            var okLink = $('<a>')

            $('#file-' + this.id + ' a.editIcon')
                .removeClass('editIcon')
                .addClass('okIcon')
                .prop('href', '#')
                .click(function () {
                    var newName = $('#file-' + me.id + ' input').val();
                    me.setName(newName);
                });

            $('#file-' + this.id + ' a.removeIcon')
                .removeClass('removeIcon')
                .addClass('cancelIcon')
                .prop('href', '#')
                .click(function () {
                    me.cancelEditMode();
                });
            editableText.focus().select();
        }

        public setName(newName: string): void {
            var me = this;
            this.nodeName = newName;
            if (this.id == -1) {
                //sendToServer
                serverHub.addNode(this.getParent().getId(), newName, this.isFolder());
            }

            var textSpan = $('<span/>')
                .addClass('text')
                .text(newName);
            var openLink = $('<a>')
                .prop('href', '#')
                .addClass('openLink')
                .append(textSpan)
                .click(function () {
                    fileManagement.setAndShowActualNode(me);
                });
            $('#file-' + this.getId() + ' input').replaceWith(openLink);

            $('#file-' + this.getId() + ' a.okIcon')
                .removeClass('okIcon')
                .addClass('editIcon')
                .prop('href', '#')
                .click(function () {
                    me.setFilenameEditable();
                });

            $('#file-' + this.getId() + ' a.cancelIcon')
                .removeClass('cancelIcon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    me.deleteFile();
                });
            fileManagement.addMode = false;
        }

        public cancelEditMode(): void {
            var me = this;
            this.setName(this.getNodename());

            $('#file-' + this.getId() + ' a.cancelIcon')
                .removeClass('cancelIcon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    me.deleteFile();
                });
        }

        public deleteFile(): void {
            var parent = this.getParent();
            var index = parent.getChilds().indexOf(this);
            if (index != -1) {
                parent.getChilds().splice(index, 1);
            }
            if (parent == null) {
                parent = fileManagement.rootNode;
            }
            fileManagement.setAndShowActualNode(parent);
            fileManagement.exitAddMode();
        }

        public asHTML(): JQuery {
            var me = this;
            var nodeJQuery = $('<div>')
                .addClass('file-node')
                .prop('id', 'file-' + this.id);
            var imgSpan = $('<span/>')
                .addClass('icon')
                .addClass('fileIcon');
            if (this.isFolder()) {
                imgSpan.addClass('folderIcon');
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
                    Stebs.fileManagement.setAndShowActualNode(me);
                });
            nodeJQuery.append(openLink);
            var editJQuery = $('<a>')
                .addClass('icon')
                .addClass('editIcon')
                .prop('href', '#')
                .click(function () {
                    me.setFilenameEditable();
                });
            nodeJQuery.append(editJQuery);
            var deleteJQuery = $('<a>')
                .addClass('icon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    me.deleteFile();
                });
            nodeJQuery.append(deleteJQuery);
            return nodeJQuery;
        }
    };

    export var fileManagement = {
        rootNode: new FileNode(0, 'root', null, true, []),
        actualNode: <FileNode>null,
        addMode: false,

        init() {
            fileManagement.actualNode = fileManagement.rootNode;

            /*for Testing */
            var inner = new FileNode(1, 'inner', fileManagement.rootNode, true, []);
            fileManagement.rootNode.appendChild(inner);
            var inner2 = new FileNode(2, 'inner2', inner, false, []);
            inner.appendChild(inner2);

            fileManagement.setAndShowActualNode(fileManagement.actualNode);

            $('#open').click(fileManagement.toggleFileManager);

            $('#new').click(fileManagement.newFile);

            $('#save').click(() => console.log('save called'));

            $('#addFile').click(function () {
                var newNode = new FileNode(-1, 'new File', fileManagement.actualNode, false, []);
                fileManagement.addFileOrFolder(newNode);
            });
            $('#addFolder').click(function () {
                var newNode = new FileNode(-1, 'new Folder', fileManagement.actualNode, true, []);
                fileManagement.addFileOrFolder(newNode);
            });
        },

        /**
         * Shows file manager view.
         */
        openFileManager() {
            $('#fileSystem').show();
        },

        /**
         * Opens the file manager, if it was closed.
         * Closes the file manager otherwise.
         */
        toggleFileManager() {
            $('#fileSystem').toggle();
        },

        /**
         * Opens the file manager with the intention to create a new file in the current folger.
         */
        newFile() {
            var newNode = new FileNode(-1, 'new File', fileManagement.actualNode, false, []);
            fileManagement.addFileOrFolder(newNode);
            fileManagement.openFileManager();
        },

        addFileOrFolder(fileNode: FileNode) {
            if (!fileManagement.addMode) {
                var actualNode = fileManagement.actualNode;
                actualNode.appendChild(fileNode);
                fileManagement.setAndShowActualNode(actualNode);
                fileManagement.addMode = true;
                fileNode.setFilenameEditable();
            }
        },
        setAndShowActualNode(fileNode: FileNode) {
            if (fileNode.isFolder()) {
                fileManagement.actualNode = fileNode;
                fileManagement.showFileManagement(fileNode);
                fileManagement.showActualPath();
            } else {
                //TODO send to load file
                console.log("loadFile");
                var fileContent = serverHub.getFileContent(fileNode.getId()).then(function (fileContent: string) {
                    $('#filename').text(fileNode.getNodename());
                    $('#fileSystem').toggle();
                    Stebs.codeEditor.getDoc().setValue(fileContent);
                });
            }
        },
        showFileManagement(fileNode: FileNode) {
            $('#files').empty();
            var childrenAsJQuery: JQuery[] = fileNode.asFolderHTML();
            for (var i = 0; i < childrenAsJQuery.length; i++) {
                $('#files').append(childrenAsJQuery[i]);
            }
        },
        showActualPath() {
            var links: JQuery[] = [];
            $('#folderPath').empty()
            var actualNode = fileManagement.actualNode;
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
            var rootNode = fileManagement.rootNode;
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
        },

        exitAddMode() {
            if (fileManagement.addMode) {
                var toDelete = fileManagement.actualNode.getById(-1);
                console.log(toDelete);
                if (toDelete != null) {
                    toDelete.deleteFile();
                }
                fileManagement.addMode = false;
            }
        }

    }

}