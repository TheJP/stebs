using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stebs5.Models;
using Stebs5Model;
using System.Security.Principal;

namespace Stebs5
{
    /// <summary>
    /// Manager, that provides methods to access the stebs filesystem.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Creates a new node with the given data.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="parentId"></param>
        /// <param name="nodeName"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        FileSystemViewModel AddNode(IPrincipal user, long parentId, string nodeName, bool isFolder);
        /// <summary>
        /// Changes the name of an existing node.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="nodeId"></param>
        /// <param name="newNodeName"></param>
        /// <returns></returns>
        FileSystemViewModel ChangeNodeName(IPrincipal user, long nodeId, string newNodeName);
        /// <summary>
        /// Deletes the node with the given id.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        FileSystemViewModel DeleteNode(IPrincipal user, long nodeId);
        /// <summary>
        /// Returns the file system of the given user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FileSystemViewModel GetFileSystem(IPrincipal user);
        /// <summary>
        /// Returns the code stored in a file node.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        string GetFileContent(IPrincipal user, long fileId);
        /// <summary>
        /// Save the content of the given file to the given content.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileId"></param>
        /// <param name="fileContent"></param>
        void SaveFileContent(IPrincipal user, long fileId, string fileContent);
    }
}
