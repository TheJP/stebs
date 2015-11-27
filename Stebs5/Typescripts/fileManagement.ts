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

        public setNewName(newName: string) {
            this.nodeName = newName;
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
                    Stebs.fileManagement.setFileEditable(me);
                });
            nodeJQuery.append(editJQuery);
            var deleteJQuery = $('<a>')
                .addClass('icon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    Stebs.fileManagement.deleteFile(me);
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

            $('#open').click(function () {
                $('#fileSystem').show();
            });

            $('#new').click(function () {
                $('#fileSystem').show();
                var newNode = new FileNode(-1, 'new File', fileManagement.actualNode, false, []);
                fileManagement.addFileOrFolder(newNode);
            });

            $('#save').click(function () {
                console.log("save called");
            });
            $('#addFile').click(function () {
                var newNode = new FileNode(-1, 'new File', fileManagement.actualNode, false, []);
                fileManagement.addFileOrFolder(newNode);
            });
            $('#addFolder').click(function () {
                var newNode = new FileNode(-1, 'new Folder', fileManagement.actualNode, true, []);
                fileManagement.addFileOrFolder(newNode);
            });
        },

        addFileOrFolder(fileNode: FileNode) {
            if (!fileManagement.addMode) {
                var actualNode = fileManagement.actualNode;
                actualNode.appendChild(fileNode);
                fileManagement.setAndShowActualNode(actualNode);
                fileManagement.addMode = true;
                fileManagement.setFileEditable(fileNode);
            }
        },
        setAndShowActualNode(fileNode: FileNode) {
            if (fileNode.isFolder()) {
                fileManagement.actualNode = fileNode;
                fileManagement.showFileManagement(fileNode);
                fileManagement.showActualPath();
            } else {
                //hideView
                $('#fileSystem').hide();
                console.log('openfile');
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

        setFileEditable(file: FileNode): void {
            var editableText = $('<input>')
                .prop('type', 'text')
                .val($('#file-' + file.getId() + ' a.openLink').text());
            $('#file-' + file.getId() + ' a.openLink').replaceWith(editableText);
            var okLink = $('<a>')

            $('#file-' + file.getId() + ' a.editIcon')
                .removeClass('editIcon')
                .addClass('okIcon')
                .prop('href', '#')
                .click(function () {
                    var newName = $('#file-' + file.getId() + ' input').val();
                    fileManagement.setName(newName, file);
                });

            $('#file-' + file.getId() + ' a.removeIcon')
                .removeClass('removeIcon')
                .addClass('cancelIcon')
                .prop('href', '#')
                .click(function () {
                    fileManagement.cancelEditMode(file);
                });
            editableText.focus().select();
        },

        setName(newName: string, file: FileNode) {
            file.setNewName(newName);
            //sendToServer
            if (file.getId() == -1) {
                if (file.isFolder()) {
                    $.connection.stebsHub.server.addFolder(file.getParent().getId(), newName);
                } else {
                    $.connection.stebsHub.server.addFile(file.getParent().getId(), newName);
                }
            } else {

            }

            var textSpan = $('<span/>')
                .addClass('text')
                .text(newName);
            var openLink = $('<a>')
                .prop('href', '#')
                .addClass('openLink')
                .append(textSpan)
                .click(function () {
                    fileManagement.setAndShowActualNode(file);
                });
            $('#file-' + file.getId() + ' input').replaceWith(openLink);

            $('#file-' + file.getId() + ' a.okIcon')
                .removeClass('okIcon')
                .addClass('editIcon')
                .prop('href', '#')
                .click(function () {
                    fileManagement.setFileEditable(file);
                });

            $('#file-' + file.getId() + ' a.cancelIcon')
                .removeClass('cancelIcon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    fileManagement.deleteFile(file);
                });
            fileManagement.addMode = false;
        },

        cancelEditMode(file: FileNode): void {
            fileManagement.setName(file.getNodename(), file);

            $('#file-' + file.getId() + ' a.cancelIcon')
                .removeClass('cancelIcon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    fileManagement.deleteFile(file);
                });
        },

        deleteFile(file: FileNode): void {
            var parent = file.getParent();
            var index = parent.getChilds().indexOf(file);
            if (index != -1) {
                parent.getChilds().splice(index, 1);
            }
            if (parent == null) {
                parent = fileManagement.rootNode;
            }
            fileManagement.setAndShowActualNode(parent);
            fileManagement.exitAddMode();
        },

        exitAddMode() {
            if (fileManagement.addMode) {
                var toDelete = fileManagement.actualNode.getById(-1);
                console.log(toDelete);
                if (toDelete != null) {
                    fileManagement.deleteFile(toDelete);
                }
                fileManagement.addMode = false;
            }
        }
    }

}