using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class Folder : IFileSystemNode
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<IFileSystemNode> Children { get; set; }
    }
}
