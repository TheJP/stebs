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
        string GetFileContent(IPrincipal user, long fileId);
        FileSystemViewModel AddNode(IPrincipal user, long parentId, string nodeName, bool isFolder);
        FileSystemViewModel ChangeNodeName(IPrincipal user, long nodeId, string newNodeName, bool isFolder);
        FileSystemViewModel DeleteNode(IPrincipal user, long nodeId, bool isFolder);
        FileSystemViewModel GetFileSystem(IPrincipal user);
        void SaveFileContent(IPrincipal user, long fileId, string fileContent);
    }
}
