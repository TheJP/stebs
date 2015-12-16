using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class StebsDbContext : IdentityDbContext<StebsUser>
    {
        public DbSet<FileSystem> FileSystems { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<File> Files { get; set; }
    }
}
