using System.ComponentModel.DataAnnotations;

namespace Stebs5Model
{
    public abstract class FileSystemNode
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual FileSystem FileSystem { get; set; }
        public virtual Folder Folder { get; set; }
    }
}