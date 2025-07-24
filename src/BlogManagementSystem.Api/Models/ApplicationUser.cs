using Microsoft.AspNetCore.Identity;

namespace BlogManagementSystem.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
    }
}
