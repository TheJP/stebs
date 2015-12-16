using System.ComponentModel.DataAnnotations;

namespace Stebs5Model
{
    public interface IFileSystemNode
    {
        [Key]
        long Id { get; set; }
        string Name { get; set; }
    }
}