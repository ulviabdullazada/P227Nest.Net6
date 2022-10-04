using Microsoft.AspNetCore.Identity;

namespace Nest.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
