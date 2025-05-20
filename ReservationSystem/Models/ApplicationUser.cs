using Microsoft.AspNetCore.Identity;

namespace ReservationSystem.Models
{
    // This class represents a user in the application.
    // It inherits from IdentityUser, which provides properties for user management.
    public class ApplicationUser : IdentityUser
    {
        // Indicates whether the user account is active
        public bool IsActive { get; set; } = true;
    }
}
