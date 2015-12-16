using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class FileSystem
    {
        [Key]
        public long Id { get; set; }
        public virtual IFileSystemNode Root { get; set; }
        public virtual ICollection<IFileSystemNode> Nodes { get; set; }
    }
}
    