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
        constructor(Id: number, Name: string) {
            super(Id, Name);
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
        rootNode: new Folder(0, 'Root', []),
        actualFolder: <Folder>null,
        openedFile: <File>null,
        addMode: false,

        /**
         * Initialize the FileMangement
         */
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
                var newNode = new File(-1, 'new File');
                fileManagement.addNode(newNode);
            });
            $('#addFolder').click(function () {
                var newNode = new Folder(-1, 'new Folder', []);
                fileManagement.addNode(newNode);
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
            } else {
                console.log("save new file");
                //This prevents the native save dialog from showin when using prompt()
                setTimeout(() => {
                    var fileName = prompt("Enter file name", "New File"); //TODO: Improve this input
                    if (fileName) {
                        Stebs.serverHub.addNode(fileManagement.actualFolder.Id, fileName, false).then(fileSystem => {
                            //TODO: Improve handling: File Replacing / Unique filenames
                            fileManagement.reloadFileManagement(fileSystem);
                            var nodes = fileManagement.actualFolder.Children.filter(node => node.Name == fileName);
                            if (nodes.length < 1) { return; }
                            var node = nodes[nodes.length - 1];
                            $('#filename').text(node.Name);
                            fileManagement.openedFile = <File>node;
                            fileManagement.saveFile();
                        });
                    }
                }, 0);
            }
        },

        /**
         * Create new file and Open FileManagement
         */
        newFile() {
            fileManagement.openFileManager();
            var newNode = new File(-1, 'new File');
            fileManagement.addNode(newNode);
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

        /**
         * Add a Node to the Filesystem at the actualNode.
         * (will not be saved to the server until new name is defined)
         * @param node The new node.
         */
        addNode(node: Node) {
            if (!fileManagement.addMode) {
                fileManagement.addMode = true;
                var actualNode = fileManagement.actualFolder;
                actualNode.Children.push(node);
                fileManagement.showFileManagement(actualNode);
                fileManagement.setFilenameEditable(node);
            }
        },

        /**
         * If node is a Folder the filesystem will be reloaded with the new folder, else the file will be loaded.
         * @param node The node to load.
         */
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

        /**
         * Clear and reinserts the Filemanagement at the given position into $('#files').
         * @param node The node position.
         */
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

        /**
         * Search recursively the parentfolder of the given folder.
         * @param startFolder the startFolder (normaly start with root).
         * @param folder the folder to search.
         */
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
 
        /**
         * Insert the actual path into the FileManagement view.
         */
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

        /**
         * Convert the node into a JQuery.
         * @param node The node to convert.
         */
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

        /**
         * Change node name to editable.
         * @param node The node to change.
         */
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

        /**
         * Check if node is a folder.
         * @param node The node to check.
         */
        nodeIsFolder(node: Node): boolean {
            return typeof (<Folder>node).Children !== 'undefined';
        }
    }
}
