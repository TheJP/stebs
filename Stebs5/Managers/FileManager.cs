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
using System.Text.RegularExpressions;

namespace Stebs5
{
    public class FileManager : IFileManager
    {
        /// <summary>
        /// Returns the filesystem of the given user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private FileSystem LoadFileSystem(string userId, StebsDbContext db) => db.Users
            .Where(u => u.Id == userId)
            .Include(u => u.FileSystem)
            .Select(u => u.FileSystem)
            .Include(fileSystem => fileSystem.Nodes)
            .FirstOrDefault();
        private FileSystem LoadFileSystem(IPrincipal user, StebsDbContext db) => LoadFileSystem(user.Identity.GetUserId(), db);

        /// <summary>
        /// Validates the given node name.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns>True if the node name was valid, false otherwise.</returns>
        private bool ValideNodeName(string nodeName) => nodeName.Length > 0 && !Regex.IsMatch(nodeName, @"[^\w_\-\. ]");

        public FileSystemViewModel GetFileSystem(IPrincipal user)
        {
            using (var db = new StebsDbContext())
            {
                return LoadFileSystem(user, db)?.ToViewModel();
            }
        }

        public FileSystemViewModel AddNode(IPrincipal user, long parentId, string nodeName, bool isFolder)
        {
            using (var db = new StebsDbContext())
            {
                //Validate input and get necessary information
                var fileSystem = LoadFileSystem(user, db);
                var parent = fileSystem.Nodes.FirstOrDefault(folder => folder.Id == parentId);
                if (parent != null && parent is Folder && ValideNodeName(nodeName))
                {
                    //Create node
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
                return fileSystem?.ToViewModel();
            }
        }

        public FileSystemViewModel ChangeNodeName(IPrincipal user, long nodeId, string newNodeName)
        {
            using(var db = new StebsDbContext())
            {
                var fileSystem = LoadFileSystem(user, db);
                var node = fileSystem.Nodes.FirstOrDefault(n => n.Id == nodeId);
                if(node != null && ValideNodeName(newNodeName))
                {
                    node.Name = newNodeName;
                    db.SaveChanges();
                }
                return fileSystem?.ToViewModel();
            }
        }

        public FileSystemViewModel DeleteNode(IPrincipal user, long nodeId)
        {
            using (var db = new StebsDbContext())
            {
                var fileSystem = LoadFileSystem(user, db);
                var node = fileSystem.Nodes.FirstOrDefault(n => n.Id == nodeId);
                //Only delete folders if they're empty
                var validFolder = (!(node is Folder) || !(node as Folder).Children.Any());
                //The root folder will not be deleted
                if (node != null && validFolder && fileSystem.Root.Id != node.Id)
                {
                    node.Folder.Children.Remove(node);
                    fileSystem.Nodes.Remove(node);
                    if(node is Folder) { db.Folders.Remove(node as Folder); }
                    else if(node is File) { db.Files.Remove(node as File); }
                    db.SaveChanges();
                }
                return fileSystem?.ToViewModel();
            }
        }

        public string GetFileContent(IPrincipal user, long fileId)
        {
            using (var db = new StebsDbContext())
            {
                var fileSystem = LoadFileSystem(user, db);
                var node = fileSystem.Nodes.FirstOrDefault(n => n.Id == fileId);
                if (node != null && node is File)
                {
                    return (node as File).Content ?? "";
                }
                return "Invalid file";
            }
        }

        public void SaveFileContent(IPrincipal user, long fileId, string fileContent)
        {
            using (var db = new StebsDbContext())
            {
                var fileSystem = LoadFileSystem(user, db);
                var node = fileSystem.Nodes.FirstOrDefault(n => n.Id == fileId);
                if (node != null && node is File)
                {
                    (node as File).Content = fileContent;
                    db.SaveChanges();
                }
            }
        }
    }
}