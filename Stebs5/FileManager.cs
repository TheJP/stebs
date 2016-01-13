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

namespace Stebs5
{
    public class FileManager : IFileManager
    {
        private Tuple<string, StebsDbContext> DBAndId
        {
            get
            {
                var context = HttpContext.Current.GetOwinContext();
                var userId = context.Authentication.User?.Identity?.GetUserId();
                var db = context.Get<StebsDbContext>();
                return Tuple.Create(userId, db);
            }
        }
        private FileSystemViewModel LoadFileSystem(string userId, StebsDbContext db) =>
            db.Users
                .Where(user => user.Id == userId)
                .Select(user => user.FileSystem)
                .Include("Nodes")
                .FirstOrDefault()
                ?.ToViewModel();

        public FileSystemViewModel AddNode(long parentId, string nodeName, bool isFolder)
        {
            var access = DBAndId;
            var db = access.Item2;
            var fileSystem = db.Users
                .Where(user => user.Id == access.Item1)
                .Select(user => user.FileSystem)
                .First();
            var parent = fileSystem.Nodes.FirstOrDefault(folder => folder.Id == parentId);
            if (parent != null && parent is Folder)
            {
                if (isFolder)
                {
                    var node = new Folder() { Folder = parent as Folder, FileSystem = fileSystem, Name = nodeName };
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
            return LoadFileSystem(access.Item1, db);
        }

        public FileSystemViewModel ChangeNodeName(long nodeId, string newNodeName, bool isFolder)
        {
            throw new NotImplementedException();
        }

        public FileSystemViewModel DeleteNode(long nodeId, bool isFolder)
        {
            throw new NotImplementedException();
        }

        public string GetFileContent(long fileId)
        {
            throw new NotImplementedException();
        }

        public FileSystemViewModel GetFileSystem()
        {
            var access = DBAndId;
            if(access.Item1 == null) { return null; }
            return LoadFileSystem(access.Item1, access.Item2);
        }

        public void SaveFileContent(long fileId, string fileContent)
        {
            throw new NotImplementedException();
        }
    }
}