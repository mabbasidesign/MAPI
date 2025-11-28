using Microsoft.AspNetCore.Identity;

namespace MAPI.Model
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
