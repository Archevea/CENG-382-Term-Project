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
    public class IndexModel : PageModel
    {
        // Database context for accessing reservations and feedbacks
        private readonly ReservationsDbContext _context;
        // UserManager for accessing the current user
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject dependencies
        public IndexModel(ReservationsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Total number of reservations for the instructor
        public int TotalReservations { get; set; }
        // Number of pending reservations
        public int PendingReservations { get; set; }
        // Number of approved reservations
        public int ApprovedReservations { get; set; }
        // Number of rejected reservations
        public int RejectedReservations { get; set; }

        // List of the most recent reservations
        public List<Reservation> RecentReservations { get; set; } = new();
        // List of the most recent feedbacks
        public List<Feedback> RecentFeedbacks { get; set; } = new();

        // Handles GET requests to display the instructor dashboard
        public async Task OnGetAsync()
        {
            // Get the current instructor user
            var user = await _userManager.GetUserAsync(User);

            // Query all reservations for the current instructor (excluding cancelled)
            var reservations = _context.Reservations
                .Include(r => r.Classroom)
                .Where(r => r.InstructorId == user.Id && !r.IsCancelled);

            // Calculate reservation statistics
            TotalReservations = await reservations.CountAsync();
            PendingReservations = await reservations.CountAsync(r => r.Status == "Pending");
            ApprovedReservations = await reservations.CountAsync(r => r.Status == "Approved");
            RejectedReservations = await reservations.CountAsync(r => r.Status == "Rejected");

            // Get the 5 most recent reservations
            RecentReservations = await reservations
                .OrderByDescending(r => r.StartTime)
                .Take(5)
                .ToListAsync();

            // Get the 5 most recent feedbacks by the instructor
            RecentFeedbacks = await _context.Feedbacks
                .Include(f => f.Classroom)
                .Where(f => f.InstructorId == user.Id)
                .OrderByDescending(f => f.SubmittedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
