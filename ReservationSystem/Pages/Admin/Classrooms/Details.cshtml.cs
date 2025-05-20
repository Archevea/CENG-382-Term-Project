using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Classrooms
{
    // PageModel for displaying classroom details, feedback, and weekly reservations
    public class DetailsModel : PageModel
    {
        // Database context for accessing data
        private readonly ReservationsDbContext _context;

        // Constructor to inject the database context
        public DetailsModel(ReservationsDbContext context)
        {
            _context = context;
        }

        // Classroom entity to display
        public Classroom Classroom { get; set; } = default!;
        // List of feedbacks for the classroom (paginated)
        public List<Feedback> Feedbacks { get; set; } = new();
        // List of reservations for the current week
        public List<Reservation> WeeklyReservations { get; set; } = new();

        // Current feedback page number
        public int CurrentPage { get; set; }
        // Total number of feedback pages
        public int TotalPages { get; set; }
        // Number of feedbacks per page
        public const int PageSize = 5;

        // Handles GET requests to display classroom details, feedback, and weekly reservations
        public async Task<IActionResult> OnGetAsync(int? id, int page = 1)
        {
            // If no classroom ID is provided, return 404
            if (id == null)
                return NotFound();

            // Retrieve classroom by ID
            Classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == id);
            if (Classroom == null)
                return NotFound();

            // Set current feedback page
            CurrentPage = page;

            // Get total feedback count for pagination
            var totalCount = await _context.Feedbacks
                .Where(f => f.ClassroomId == id)
                .CountAsync();

            // Calculate total number of feedback pages
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated feedbacks for the classroom
            Feedbacks = await _context.Feedbacks
                .Where(f => f.ClassroomId == id)
                .OrderByDescending(f => f.SubmittedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Calculate the start and end of the current week (Monday to Sunday)
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            // Retrieve reservations for the classroom in the current week (excluding cancelled)
            WeeklyReservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Include(r => r.Term)
                .Where(r => r.ClassroomId == id && r.StartTime >= startOfWeek && r.EndTime < endOfWeek && !r.IsCancelled)
                .OrderBy(r => r.StartTime)
                .ToListAsync();

            // Return the page with all data
            return Page();
        }
    }
}
