using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class Folder : FileSystemNode
    {
        [Key]
        public override long Id { get; set; }
        public override string Name { get; set; }
        [InverseProperty("Folder")]
        public virtual ICollection<FileSystemNode> Children { get; set; } = new HashSet<FileSystemNode>();
    }
}
