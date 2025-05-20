using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservationSystem.Models;

namespace ReservationSystem.Pages.Admin.InstructorAccounts
{
    // Only allow Admins to access this page
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        // UserManager for managing user accounts
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject UserManager
        public DeleteModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Property to bind the instructor user to the page
        [BindProperty]
        public ApplicationUser Instructor { get; set; }

        // Handles GET requests to display the deactivate confirmation page
        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Find the instructor by ID
            Instructor = await _userManager.FindByIdAsync(id);

            // If not found or not an instructor, return 404
            if (Instructor == null || !await _userManager.IsInRoleAsync(Instructor, "Instructor"))
                return NotFound();

            // Show the confirmation page
            return Page();
        }

        // Handles POST requests to deactivate the instructor
        public async Task<IActionResult> OnPostAsync(string id)
        {
            // Find the instructor by ID
            var instructor = await _userManager.FindByIdAsync(id);

            // If not found or not an instructor, return 404
            if (instructor == null || !await _userManager.IsInRoleAsync(instructor, "Instructor"))
                return NotFound();

            // Set the instructor as inactive (deactivate)
            instructor.IsActive = false;
            await _userManager.UpdateAsync(instructor);

            // Redirect to the instructor list page after deactivation
            return RedirectToPage("./Index");
        }
    }
}
