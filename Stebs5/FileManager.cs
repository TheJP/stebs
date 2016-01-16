using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using Stebs5.Models;
using Microsoft.Owin;
using Stebs5Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Security.Principal;

namespace Stebs5
{
    public class FileManager : IFileManager
    {
        private FileSystemViewModel LoadFileSystem(IPrincipal user, StebsDbContext db) =>
            db.Users
                .Where(u => u.UserName == user.Identity.Name)
                .Include(u => u.FileSystem)
                .Select(u => u.FileSystem)
                .Include(fileSystem => fileSystem.Nodes)
                .FirstOrDefault()
                ?.ToViewModel();

        public FileSystemViewModel AddNode(IPrincipal user, long parentId, string nodeName, bool isFolder)
        {
            using (var db = new StebsDbContext())
            {
                var fileSystem = db.Users
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.FileSystem)
                    .First();
                var parent = fileSystem.Nodes.FirstOrDefault(folder => folder.Id == parentId);
                if (parent != null && parent is Folder)
                {
                    if (isFolder)
                    {
                        var node = new Folder() { FileSystem = fileSystem, Folder = parent as Folder, Name = nodeName };
                        fileSystem.Nodes.Add(node);
                        db.Folders.Add(node);
                    }
                    else
                    {
                        var node = new File() { FileSystem = fileSystem, Folder = parent as Folder, Name = nodeName };
                        fileSystem.Nodes.Add(node);
                        db.Files.Add(node);
                    }
                    db.SaveChanges();
                }
                return LoadFileSystem(user, db);
            }
        }

        public FileSystemViewModel ChangeNodeName(IPrincipal user, long nodeId, string newNodeName, bool isFolder)
        {
            throw new NotImplementedException();
        }

        public FileSystemViewModel DeleteNode(IPrincipal user, long nodeId, bool isFolder)
        {
            throw new NotImplementedException();
        }

        public string GetFileContent(IPrincipal user, long fileId)
        {
            throw new NotImplementedException();
        }

        public FileSystemViewModel GetFileSystem(IPrincipal user)
        {
            using (var db = new StebsDbContext())
            {
                return LoadFileSystem(user, db);
            }
        }

        public void SaveFileContent(IPrincipal user, long fileId, string fileContent)
        {
            throw new NotImplementedException();
        }
    }
}