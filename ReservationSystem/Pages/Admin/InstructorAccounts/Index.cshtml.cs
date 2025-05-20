using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Pages.Admin.InstructorAccounts
{
    // Only allow Admins to access this page
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        // UserManager for managing user accounts
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject UserManager
        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ViewModel for displaying instructor info in the list
        public class InstructorViewModel
        {
            public ApplicationUser User { get; set; } = default!;
            public string Role { get; set; } = "";
        }

        // List of instructors to display in the view
        public List<InstructorViewModel> Instructors { get; set; } = new();
        // Search term entered by the user
        public string? SearchTerm { get; set; }
        // Current page number for pagination
        public int CurrentPage { get; set; }
        // Total number of pages for pagination
        public int TotalPages { get; set; }

        // Number of instructors to display per page
        private const int PageSize = 10;

        // Handles GET requests to display the instructor list with search and pagination
        public async Task OnGetAsync(string? search, int page = 1)
        {
            SearchTerm = search;
            CurrentPage = page;

            // Get all users from the database
            var allUsers = await _userManager.Users.ToListAsync();
            var filtered = allUsers.AsQueryable();

            // Filter users by search term if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(u =>
                    (u.Email != null && u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (u.UserName != null && u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            // Get total count for pagination
            var totalCount = filtered.Count();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated instructor list
            var pageUsers = filtered
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // For each user, get their roles and add to the instructor list
            foreach (var user in pageUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Instructors.Add(new InstructorViewModel
                {
                    User = user,
                    Role = string.Join(", ", roles)
                });
            }
        }

        // Handles POST requests to activate a deactivated instructor
        public async Task<IActionResult> OnPostActivateAsync(string id)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Set the user as active
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            // Redirect to the instructor list page after activation
            return RedirectToPage();
        }
    }
}
