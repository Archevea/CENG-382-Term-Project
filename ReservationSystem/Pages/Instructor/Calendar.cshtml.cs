// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;
using System.Text.Json;

namespace ReservationSystem.Pages.Instructor
{
    // Only allow Instructors to access this page
    [Authorize(Roles = "Instructor")]
    public class CalendarModel : PageModel
    {
        // Database context for accessing reservations and classrooms
        private readonly ReservationsDbContext _context;
        // UserManager for accessing the current user
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject dependencies
        public CalendarModel(ReservationsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List of classrooms for the dropdown in the reservation form
        public List<Classroom> ClassroomList { get; set; } = new();

        // Properties bound to the reservation form
        [BindProperty]
        public string ReservationDate { get; set; } = string.Empty;

        [BindProperty]
        public TimeSpan StartTime { get; set; }

        [BindProperty]
        public TimeSpan EndTime { get; set; }

        [BindProperty]
        public int ClassroomId { get; set; }

        // Handles GET requests to display the calendar and classroom list
        public async Task OnGetAsync()
        {
            ClassroomList = await _context.Classrooms.ToListAsync();
        }

        // Handles AJAX GET requests to provide reservation events for the calendar
        public async Task<JsonResult> OnGetReservationsAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var reservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Where(r => r.InstructorId == user.Id && !r.IsCancelled)
                .Select(r => new
                {
                    id = r.Id,
                    title = $"{r.Classroom.Name} ({r.StartTime:HH\\:mm}-{r.EndTime:HH\\:mm})",
                    start = r.StartTime,
                    end = r.EndTime,
                    color = r.Status == "Approved" ? "#28a745" :
                            r.Status == "Rejected" ? "#dc3545" :
                            "#ffc107",
                    extendedProps = new
                    {
                        classroomId = r.ClassroomId
                    }
                })
                .ToListAsync();

            return new JsonResult(reservations);
        }

        // Handles POST requests to create a new reservation
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // Validate reservation date
            if (!DateTime.TryParse(ReservationDate, out var date))
            {
                ModelState.AddModelError(string.Empty, "Invalid date.");
                await OnGetAsync();
                return Page();
            }

            // Calculate start and end DateTime
            var startDateTime = date.Date + StartTime;
            var endDateTime = date.Date + EndTime;

            // Ensure end time is after start time
            if (endDateTime <= startDateTime)
            {
                ModelState.AddModelError(string.Empty, "End time must be after start time.");
                await OnGetAsync();
                return Page();
            }

            // Check if the date is within an active term
            var term = await _context.Terms.FirstOrDefaultAsync(t =>
                t.StartDate <= date && t.EndDate >= date);

            if (term == null)
            {
                ModelState.AddModelError(string.Empty, "There is no active term.");
                await OnGetAsync();
                return Page();
            }

            // Check if the selected date is a public holiday
            if (await CheckIfPublicHoliday(date))
            {
                ModelState.AddModelError(string.Empty, "Selected date is a public holiday.");
                await OnGetAsync();
                return Page();
            }

            // Check for time conflicts with existing reservations
            var conflict = await _context.Reservations.AnyAsync(r =>
                r.ClassroomId == ClassroomId &&
                !r.IsCancelled &&
                r.StartTime < endDateTime &&
                r.EndTime > startDateTime);

            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "Selected time conflicts with another reservation.");
                await OnGetAsync();
                return Page();
            }

            // Create and save the new reservation
            var reservation = new Reservation
            {
                InstructorId = user.Id,
                ClassroomId = ClassroomId,
                TermId = term.Id,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Status = "Pending",
                IsCancelled = false
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reservation request submitted.";
            return RedirectToPage();
        }

        // Checks if a given date is a public holiday using an external API
        private async Task<bool> CheckIfPublicHoliday(DateTime date)
        {
            try
            {
                var client = new HttpClient();
                string url = $"https://date.nager.at/api/v3/PublicHolidays/{date.Year}/TR";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadAsStringAsync();
                var holidays = JsonSerializer.Deserialize<List<HolidayDto>>(json);
                return holidays!.Any(h => DateTime.Parse(h.Date).Date == date.Date);
            }
            catch
            {
                return false;
            }
        }

        // DTO for deserializing public holiday API responses
        private class HolidayDto
        {
            public string Date { get; set; } = "";
            public string LocalName { get; set; } = "";
        }
    }
}
