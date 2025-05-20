using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Classrooms
{
    // PageModel for editing a classroom
    public class EditModel : PageModel
    {
        // Database context for accessing classrooms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public EditModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Property to bind form data to the Classroom model
        [BindProperty]
        public Classroom Classroom { get; set; } = default!;

        // Handles GET requests to display the edit form
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the classroom by ID
            var classroom =  await _context.Classrooms.FirstOrDefaultAsync(m => m.Id == id);
            if (classroom == null)
            {
                return NotFound();
            }
            Classroom = classroom;
            return Page();
        }

        // Handles POST requests to update the classroom
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form data is invalid, redisplay the form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Mark the classroom entity as modified
            _context.Attach(Classroom).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the classroom no longer exists, return 404
                if (!ClassroomExists(Classroom.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to the classroom list page after successful edit
            return RedirectToPage("./Index");
        }

        // Helper method to check if a classroom exists by ID
        private bool ClassroomExists(int id)
        {
            return _context.Classrooms.Any(e => e.Id == id);
        }
    }
}
