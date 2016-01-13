using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stebs5.Models;

namespace Stebs5
{
    /// <summary>
    /// Manager, that provides methods to access the stebs filesystem.
    /// </summary>
    public interface IFileManager
    {
        FileSystemViewModel AddNode(long parentId, string nodeName, bool isFolder);
        FileSystemViewModel ChangeNodeName(long nodeId, string newNodeName, bool isFolder);
        FileSystemViewModel DeleteNode(long nodeId, bool isFolder);
        FileSystemViewModel GetFileSystem();
        string GetFileContent(long fileId);
        void SaveFileContent(long fileId, string fileContent);
    }
}
