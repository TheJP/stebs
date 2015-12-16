using Microsoft.AspNet.Identity.EntityFramework;

namespace Stebs5Model
{
    public class StebsUser : IdentityUser
    {
        public virtual FileSystem FileSystem { get; set; }
    }
}