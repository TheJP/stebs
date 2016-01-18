using Stebs5Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stebs5.Models
{
    public static class FileSystemExtensions
    {
        public static FileSystemViewModel ToViewModel(this FileSystem fileSystem) => new FileSystemViewModel(fileSystem.Root.ToNodeViewModel());

        public static NodeViewModel ToNodeViewModel(this FileSystemNode node)
        {
            if(node is Folder) { return (node as Folder).ToFolderViewModel(); }
            else if(node is File) { return (node as File).ToFileViewModel(); }
            else { return null; }
        }
        public static FolderViewModel ToFolderViewModel(this Folder node) => new FolderViewModel(node.Id, node.Name, node.Children.Select(ToNodeViewModel).ToList());
        public static FileViewModel ToFileViewModel(this File node) => new FileViewModel(node.Id, node.Name);
    }
    public class FileSystemViewModel
    {
        public NodeViewModel Root { get; }
        public FileSystemViewModel(NodeViewModel root) { this.Root = root; }
    }
    public abstract class NodeViewModel
    {
        public long Id { get; }
        public string Name { get; }
        public NodeViewModel(long id, string name) { this.Id = id; this.Name = name; }
    }
    public class FolderViewModel : NodeViewModel
    {
        public IEnumerable<NodeViewModel> Children { get; }
        public FolderViewModel(long id, string name, IEnumerable<NodeViewModel> children) : base(id, name) { this.Children = children; }
    }
    public class FileViewModel : NodeViewModel
    {
        public FileViewModel(long id, string name) : base(id, name) { }
    }
}