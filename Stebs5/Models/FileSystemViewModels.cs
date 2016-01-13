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
        public static FolderViewModel ToFolderViewModel(this Folder node) => new FolderViewModel(node.Name, node.Children.Select(ToNodeViewModel));
        public static FileViewModel ToFileViewModel(this File node) => new FileViewModel(node.Name);
    }
    public class FileSystemViewModel
    {
        public NodeViewModel Root { get; }
        public FileSystemViewModel(NodeViewModel root) { this.Root = root; }
    }
    public abstract class NodeViewModel
    {
        public string Name { get; }
        public NodeViewModel(string name) { this.Name = name; }
    }
    public class FolderViewModel : NodeViewModel
    {
        public IEnumerable<NodeViewModel> Children { get; }
        public FolderViewModel(string name, IEnumerable<NodeViewModel> children) : base(name) { this.Children = children; }
    }
    public class FileViewModel : NodeViewModel
    {
        public FileViewModel(string name) : base(name) { }
    }
}