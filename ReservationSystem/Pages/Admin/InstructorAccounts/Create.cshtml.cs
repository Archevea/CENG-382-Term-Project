using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Pages.Admin.InstructorAccounts
{
    // Only allow Admins to access this page
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        // UserManager for managing user accounts
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject UserManager
        public CreateModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Email property bound to the form, required and must be a valid email
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Password property bound to the form, required and must be a password
        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Role property bound to the form, required
        [BindProperty]
        [Required]
        public string Role { get; set; } = string.Empty;

        // Handles POST requests to create a new user
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form data is invalid, redisplay the form
            if (!ModelState.IsValid)
                return Page();

            // Create a new ApplicationUser instance
            var user = new ApplicationUser
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = true, // Mark email as confirmed
                IsActive = true        // Set user as active
            };

            // Attempt to create the user with the specified password
            var result = await _userManager.CreateAsync(user, Password);

            // If user creation succeeded, add to the selected role and redirect
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role);
                return RedirectToPage("./Index");
            }

            // If there are errors, add them to the ModelState to display in the form
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            // Redisplay the form with validation errors
            return Page();
        }
    }
}
