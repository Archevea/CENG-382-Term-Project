// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages.Instructor
{
    // Only allow Instructors to access this page
    [Authorize(Roles = "Instructor")]
    public class ClassroomsModel : PageModel
    {
        // Database context for accessing classrooms, reservations, and feedbacks
        private readonly ReservationsDbContext _context;
        // UserManager for accessing the current user
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject dependencies
        public ClassroomsModel(ReservationsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List of classrooms to display in the view
        public IList<Classroom> Classrooms { get; set; } = new List<Classroom>();
        // Dictionary mapping classroom IDs to their weekly reservation schedules
        public Dictionary<int, List<WeeklyReservationView>> WeeklySchedules { get; set; } = new();

        // Properties bound to the feedback form
        [BindProperty]
        public string Comment { get; set; } = string.Empty;

        [BindProperty]
        public int? Rating { get; set; }

        [BindProperty]
        public int SelectedClassroomId { get; set; }

        // Search term entered by the user
        public string? SearchTerm { get; set; }
        // Current page number for pagination
        public int CurrentPage { get; set; }
        // Total number of pages for pagination
        public int TotalPages { get; set; }
        // Number of classrooms to display per page
        private const int PageSize = 10;

        // Handles GET requests to display the classroom list and weekly schedules
        public async Task OnGetAsync(string? search, int page = 1)
        {
            SearchTerm = search;
            CurrentPage = page;

            // Query all classrooms
            var query = _context.Classrooms.AsQueryable();

            // Filter classrooms by search term if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated classroom list
            Classrooms = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // For each classroom, get its weekly reservation schedule
            foreach (var classroom in Classrooms)
            {
                var weeklyReservations = await _context.Reservations
                    .Include(r => r.Instructor)
                    .Where(r => r.ClassroomId == classroom.Id && !r.IsCancelled && r.Status == "Approved")
                    .ToListAsync();

                var grouped = weeklyReservations
                    .Select(r => new WeeklyReservationView
                    {
                        Day = r.StartTime.DayOfWeek.ToString(),
                        Time = $"{r.StartTime:HH\\:mm} - {r.EndTime:HH\\:mm}",
                        Instructor = r.Instructor?.UserName ?? "-"
                    })
                    .OrderBy(r => r.Day)
                    .ThenBy(r => r.Time)
                    .ToList();

                WeeklySchedules[classroom.Id] = grouped;
            }
        }

        // Handles POST requests to submit classroom feedback
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // Validate feedback input
            if (string.IsNullOrWhiteSpace(Comment) || user == null || Rating == null)
                return RedirectToPage();

            // Create and save the feedback
            var feedback = new Feedback
            {
                InstructorId = user.Id,
                ClassroomId = SelectedClassroomId,
                Comment = Comment,
                Rating = Rating
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Redirect to the classroom list page after feedback submission
            return RedirectToPage();
        }

        // ViewModel for displaying weekly reservation info
        public class WeeklyReservationView
        {
            public string Day { get; set; } = "";
            public string Time { get; set; } = "";
            public string Instructor { get; set; } = "";
        }
    }
}
