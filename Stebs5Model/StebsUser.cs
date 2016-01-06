using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Stebs5Model
{
    public class StebsUser : IdentityUser
    {
        public virtual FileSystem FileSystem { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<StebsUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}