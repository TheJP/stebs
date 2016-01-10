using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class File : FileSystemNode
    {
        [Key]
        public override long Id { get; set; }
        public override string Name { get; set; }
        public virtual string Content { get; set; }
    }
}
