// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages.Admin
{
    // Only allow Admins to access this page
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        // Database context for accessing reservations, classrooms, terms, etc.
        private readonly ReservationsDbContext _context;

        // Constructor to inject the database context
        public IndexModel(ReservationsDbContext context)
        {
            _context = context;
        }

        // Total number of reservations in the system
        public int TotalReservations { get; set; }
        // Number of pending reservations
        public int PendingCount { get; set; }
        // Number of approved reservations
        public int ApprovedCount { get; set; }
        // Number of rejected reservations
        public int RejectedCount { get; set; }

        // Total number of classrooms
        public int TotalClassrooms { get; set; }

        // Name of the currently active term, or a default message if none
        public string ActiveTermName { get; set; } = "No active term";

        // List of the most recent reservations
        public List<Reservation> LatestReservations { get; set; } = new();
        // List of the most recent feedbacks
        public List<Feedback> LatestFeedbacks { get; set; } = new();

        // Handles GET requests to display the admin dashboard
        public async Task OnGetAsync()
        {
            // Get reservation statistics
            TotalReservations = await _context.Reservations.CountAsync();
            PendingCount = await _context.Reservations.CountAsync(r => r.Status == "Pending");
            ApprovedCount = await _context.Reservations.CountAsync(r => r.Status == "Approved");
            RejectedCount = await _context.Reservations.CountAsync(r => r.Status == "Rejected");

            // Get total number of classrooms
            TotalClassrooms = await _context.Classrooms.CountAsync();

            // Find the currently active term (if any)
            var today = DateTime.Today;
            var term = await _context.Terms.FirstOrDefaultAsync(t => t.StartDate <= today && t.EndDate >= today);
            if (term != null)
                ActiveTermName = $"{term.Name} ({term.StartDate:dd MMM} - {term.EndDate:dd MMM})";

            // Get the 5 most recent reservations (with classroom and instructor info)
            LatestReservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Include(r => r.Instructor)
                .OrderByDescending(r => r.StartTime)
                .Take(5)
                .ToListAsync();

            // Get the 5 most recent feedbacks (with classroom info)
            LatestFeedbacks = await _context.Feedbacks
                .Include(f => f.Classroom)
                .OrderByDescending(f => f.SubmittedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
