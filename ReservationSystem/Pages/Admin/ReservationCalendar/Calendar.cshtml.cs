// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages.Admin.ReservationCalendar
{
    // Only allow Admins to access this page
    [Authorize(Roles = "Admin")]
    public class CalendarModel : PageModel
    {
        // Database context for accessing reservations
        private readonly ReservationsDbContext _context;

        // Constructor to inject the database context
        public CalendarModel(ReservationsDbContext context)
        {
            _context = context;
        }

        // List of reservations to display in the view
        public List<ReservationViewModel> Reservations { get; set; } = new();

        // Handles GET requests to display the reservation list
        public async Task OnGetAsync()
        {
            // Retrieve all non-cancelled reservations with related classroom and instructor info
            Reservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Include(r => r.Instructor)
                .Where(r => !r.IsCancelled)
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    Instructor = r.Instructor.UserName,
                    Classroom = r.Classroom.Name,
                    Start = r.StartTime,
                    End = r.EndTime,
                    Status = r.Status
                })
                .ToListAsync();
        }

        // Handles POST requests for reservation actions (approve, reject, delete)
        public async Task<IActionResult> OnPostActionAsync([FromForm] ReservationActionRequest Request)
        {
            // Validate the request
            if (Request == null || string.IsNullOrWhiteSpace(Request.Action))
                return BadRequest("Invalid request.");

            // Find the reservation by ID
            var reservation = await _context.Reservations.FindAsync(Request.Id);
            if (reservation == null)
                return NotFound("Reservation not found.");

            // Perform the requested action
            switch (Request.Action.ToLower())
            {
                case "approve":
                    reservation.Status = "Approved";
                    break;
                case "reject":
                    reservation.Status = "Rejected";
                    break;
                case "delete":
                    reservation.IsCancelled = true;
                    break;
                default:
                    return BadRequest("Invalid action.");
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // ViewModel for displaying reservation info in the list
        public class ReservationViewModel
        {
            public int Id { get; set; }
            public string Instructor { get; set; } = "";
            public string Classroom { get; set; } = "";
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Status { get; set; } = "";
        }

        // Model for handling reservation action requests (approve, reject, delete)
        public class ReservationActionRequest
        {
            public int Id { get; set; }
            public string Action { get; set; } = "";
        }
    }
}
