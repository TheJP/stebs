using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class FileSystem
    {
        [Key]
        public long Id { get; set; }
        [Column]
        public virtual FileSystemNode Root { get; set; }
        [InverseProperty("FileSystem")]
        public virtual ICollection<FileSystemNode> Nodes { get; set; } = new HashSet<FileSystemNode>();
    }
}
    