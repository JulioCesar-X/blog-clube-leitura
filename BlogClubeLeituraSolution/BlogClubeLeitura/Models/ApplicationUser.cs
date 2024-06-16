using Microsoft.AspNetCore.Identity;

namespace BlogClubeLeitura.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
    }
}