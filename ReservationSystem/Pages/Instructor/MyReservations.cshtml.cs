// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages.Instructor
{
    // Only allow Instructors to access this page
    [Authorize(Roles = "Instructor")]
    public class MyReservationsModel : PageModel
    {
        // Database context for accessing reservations
        private readonly ReservationsDbContext _context;
        // UserManager for accessing the current user
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject dependencies
        public MyReservationsModel(ReservationsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List of reservations to display in the view
        public List<Reservation> Reservations { get; set; } = new();
        // Search term entered by the user
        public string? SearchTerm { get; set; }
        // Current page number for pagination
        public int CurrentPage { get; set; }
        // Total number of pages for pagination
        public int TotalPages { get; set; }

        // Number of reservations to display per page
        private const int PageSize = 10;

        // Handles GET requests to display the user's reservations with search and pagination
        public async Task OnGetAsync(string? search, int page = 1)
        {
            // Get the current instructor user
            var user = await _userManager.GetUserAsync(User);

            SearchTerm = search;
            CurrentPage = page;

            // Query all reservations for the current instructor
            var query = _context.Reservations
                .Include(r => r.Classroom)
                .Where(r => r.InstructorId == user.Id);

            // Filter reservations by classroom name if search term is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.Classroom.Name.Contains(search));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated reservation list, ordered by start time descending
            Reservations = await query
                .OrderByDescending(r => r.StartTime)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
