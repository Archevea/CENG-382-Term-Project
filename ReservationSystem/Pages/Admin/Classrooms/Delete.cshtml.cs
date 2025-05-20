using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Classrooms
{
    // PageModel for the Delete Classroom page
    public class DeleteModel : PageModel
    {
        // Database context for accessing classrooms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public DeleteModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Property to bind the classroom data
        [BindProperty]
        public Classroom Classroom { get; set; } = default!;

        // Handles GET requests to display the delete confirmation page
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Find the classroom by ID
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(m => m.Id == id);

            // If found, bind to the page and display
            if (classroom is not null)
            {
                Classroom = classroom;
                return Page();
            }

            // If not found, return 404
            return NotFound();
        }

        // Handles POST requests to delete the classroom
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Find the classroom by ID
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom != null)
            {
                Classroom = classroom;
                _context.Classrooms.Remove(Classroom); // Remove the classroom from the database
                await _context.SaveChangesAsync(); // Save changes
            }

            // Redirect to the classroom list page after deletion
            return RedirectToPage("./Index");
        }
    }
}
