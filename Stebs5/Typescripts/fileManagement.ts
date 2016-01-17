module Stebs {

    export class FileSystem {
        public Id: number;
        public Root: Node;
    }

    export class Node {
        public Id: number;
        public Name: string = '';

        constructor(Id: number, Name: string) {
            this.Id = Id;
            this.Name = Name;
        }
    }

    export class File extends Node {
        public Content: string;

        constructor(Id: number, Name: string, Content: string) {
            super(Id, Name);
            this.Content = Content;
        }
    }

    export class Folder extends Node {
        public Children: Node[];

        constructor(Id: number, Name: string, Children: Node[]) {
            super(Id, Name);
            this.Children = <Node[]>Children;
        }
    };

    export var fileManagement = {
        fileSystem: <FileSystem>null,
        rootNode: new Folder(0, 'root', [new File(1, 'child12', '')]),
        actualFolder: <Folder>null,
        openedFile: <File>null,
        addMode: false,

        init() {
            //Get Filesystem
            serverHub.getFileSystem().then(function (fileSystem: FileSystem) {
                fileManagement.fileSystem = fileSystem;
                fileManagement.rootNode = <Folder>fileSystem.Root;

                fileManagement.actualFolder = fileManagement.rootNode;
                fileManagement.setAndShowActualNode(fileManagement.actualFolder);
            });

            $('#open').click(fileManagement.toggleFileManager);

            $('#new').click(fileManagement.newFile);

            $('#save').click(fileManagement.saveFile);

            $('#addFile').click(function () {
                var newNode = new File(-1, 'new File', '');
                fileManagement.addFileOrFolder(newNode);
            });
            $('#addFolder').click(function () {
                var newNode = new Folder(-1, 'new Folder', []);
                fileManagement.addFileOrFolder(newNode);
            });
        },

        /**
         * Save the FileContent to the server
         */
        saveFile() {
            if (fileManagement.openedFile != null) {
                console.log('save called');
                var newSource = Stebs.codeEditor.getDoc().getValue().replace(/\r?\n/g, '\r\n').replace(/\t/g, '    ');
                serverHub.saveFileContent(fileManagement.openedFile.Id, newSource);
            }
        },

        /**
         * Create new file and Open FileManagement
         */
        newFile() {
            var newNode = new File(-1, 'new File', '');
            fileManagement.addFileOrFolder(newNode);
            fileManagement.openFileManager();
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
        * BindFunction to reload File Management
        */
        reloadFileManagement(fileSystem: FileSystem) {
            fileManagement.fileSystem = fileSystem;
            var searchFolders = function searchFolders(node: Node) {
                if (fileManagement.nodeIsFolder(node)) {
                    if (node.Id == fileManagement.actualFolder.Id) {
                        fileManagement.actualFolder = <Folder>node;
                    }
                    if (node.Id == fileManagement.rootNode.Id) {
                        fileManagement.rootNode = <Folder>node;
                    }
                    (<Folder>node).Children.forEach(searchFolders);
                }
            };
            searchFolders(fileSystem.Root);

            fileManagement.setAndShowActualNode(fileManagement.actualFolder);
        },

        addFileOrFolder(node: Node) {
            if (!fileManagement.addMode) {
                fileManagement.addMode = true;
                var actualNode = fileManagement.actualFolder;
                actualNode.Children.push(node);
                fileManagement.showFileManagement(actualNode);
                fileManagement.setFilenameEditable(node);
            }
        },
        setAndShowActualNode(node: Node) {
            if (fileManagement.nodeIsFolder(node)) {
                fileManagement.actualFolder = <Folder>node;
                fileManagement.showFileManagement(<Folder>node);
                fileManagement.showActualPath();
            } else {
                var fileContent = serverHub.getFileContent(node.Id).then(function (fileContent: string) {
                    $('#filename').text(node.Name);
                    $('#fileSystem').toggle();
                    codeEditor.getDoc().setValue(fileContent);
                    fileManagement.openedFile = <File>node;
                });
            }
        },
        showFileManagement(node: Node) {
            $('#files').empty();
            if (fileManagement.nodeIsFolder(node)) {
                var children = (<Folder>node).Children;
                for (var i = 0; i < children.length; i++) {
                    var nodeAsHtml = fileManagement.nodeToHtml(children[i]);
                    $('#files').append(nodeAsHtml);
                }
            }
        },
        getParentFolder(startFolder: Folder, folder: Folder): Folder {
            if (!fileManagement.nodeIsFolder(startFolder)) {
                console.log('startFolder has no childs');
                return null;
            }
            if (!fileManagement.nodeIsFolder(folder)) {
                console.log('folder to search has no childs');
                return null;
            }
            for (var i = 0; i < startFolder.Children.length; i++) {
                if (fileManagement.nodeIsFolder(startFolder.Children[i])) {
                    if (startFolder.Children[i].Id === folder.Id) {
                        return startFolder;
                    } else {
                        var found = fileManagement.getParentFolder(<Folder>startFolder.Children[i], folder);
                        if (found != null) {
                            return found;
                        }
                    }
                }
            }
            return null;
        },
 
        showActualPath() {
            var links: JQuery[] = [];
            var travelFolderNode: Folder = fileManagement.actualFolder;
            while (travelFolderNode.Id !== fileManagement.rootNode.Id && travelFolderNode != null) {
                if (travelFolderNode != null) {
                    var toTravel = travelFolderNode;
                    var link = $('<a>')
                        .prop('href', '#')
                        .text(travelFolderNode.Name)
                        .click(function () {
                            fileManagement.setAndShowActualNode(toTravel);
                        });
                    links.push(link);
                    travelFolderNode = fileManagement.getParentFolder(fileManagement.rootNode, travelFolderNode);
                }
            }
            if (travelFolderNode != null) {
                $('#folderPath').empty();
                $('#folderPath').append('/');
                var rootLink = $('<a>')
                    .prop('href', '#')
                    .text(travelFolderNode.Name)
                    .click(function () {
                        fileManagement.setAndShowActualNode(travelFolderNode);
                    });
                $('#folderPath').append(rootLink);
                $('#folderPath').append('/');
                for (var i = links.length - 1; i >= 0; i--) {
                    $('#folderPath').append(links[i]);
                    $('#folderPath').append('/');
                }
            }
        },

        nodeToHtml(node: Node): JQuery {
            var nodeJQuery = $('<div>')
                .addClass('file-node')
                .prop('id', 'file-' + node.Id);
            var imgSpan = $('<span/>')
                .addClass('icon')
                .addClass('fileIcon');
            if (fileManagement.nodeIsFolder(node)) {
                imgSpan.addClass('folderIcon');
            }
            nodeJQuery.append(imgSpan);

            var textSpan = $('<span/>')
                .addClass('text')
                .text(node.Name);
            var openLink = $('<a>')
                .addClass('openLink')
                .prop('href', '#')
                .append(textSpan)
                .click(function () {
                    fileManagement.setAndShowActualNode(node);
                });
            nodeJQuery.append(openLink);
            var editJQuery = $('<a>')
                .addClass('icon')
                .addClass('editIcon')
                .prop('href', '#')
                .click(function () {
                    fileManagement.setFilenameEditable(node);
                });
            nodeJQuery.append(editJQuery);
            var deleteJQuery = $('<a>')
                .addClass('icon')
                .addClass('removeIcon')
                .prop('href', '#')
                .click(function () {
                    if (node.Id != -1) {
                        console.log('delete file in backend clicked');
                        serverHub.deleteNode(node.Id, fileManagement.nodeIsFolder(node)).then(fileManagement.reloadFileManagement);
                    }
                });
            nodeJQuery.append(deleteJQuery);

            return nodeJQuery;
        },

        setFilenameEditable(node: Node) {
            var editableText = $('<input>')
                .prop('type', 'text')
                .val($('#file-' + node.Id + ' a.openLink').text());
            $('#file-' + node.Id + ' a.openLink').replaceWith(editableText);
            var okLink = $('<a>')

            $('#file-' + node.Id + ' a.editIcon')
                .removeClass('editIcon')
                .addClass('okIcon')
                .prop('href', '#')
                .click(function () {
                    var newName = $('#file-' + node.Id + ' input').val();
                    if (node.Id == -1) {
                        console.log('add new File on server clicked');
                        serverHub.addNode(fileManagement.actualFolder.Id, newName, fileManagement.nodeIsFolder(node))
                            .then(fileManagement.reloadFileManagement);
                    } else {
                        console.log('change filename on server clicked');
                        serverHub.changeNodeName(node.Id, newName, fileManagement.nodeIsFolder(node))
                            .then(fileManagement.reloadFileManagement);
                    }
                    fileManagement.addMode = false;
                });

            $('#file-' + node.Id + ' a.removeIcon')
                .removeClass('removeIcon')
                .addClass('cancelIcon')
                .prop('href', '#')
                .unbind()
                .click(function () {
                    console.log('reload without change clicked');
                    serverHub.getFileSystem().then(fileManagement.reloadFileManagement);
                    fileManagement.addMode = false;
                });
            editableText.focus().select();
        },

        nodeIsFolder(node: Node): boolean {
            return typeof (<Folder>node).Children !== 'undefined';
        }
    }
}
