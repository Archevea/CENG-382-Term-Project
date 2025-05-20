using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Terms
{
    // PageModel for deleting an academic term
    public class DeleteModel : PageModel
    {
        // Database context for accessing terms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public DeleteModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Property to bind the term to the page
        [BindProperty]
        public Term Term { get; set; } = default!;

        // Handles GET requests to display the delete confirmation page
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Find the term by ID
            var term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == id);

            // If found, bind to the page and display
            if (term is not null)
            {
                Term = term;
                return Page();
            }

            // If not found, return 404
            return NotFound();
        }

        // Handles POST requests to delete the term
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Find the term by ID
            var term = await _context.Terms.FindAsync(id);
            if (term != null)
            {
                Term = term;
                _context.Terms.Remove(Term); // Remove the term from the database
                await _context.SaveChangesAsync(); // Save changes
            }

            // Redirect to the term list page after deletion
            return RedirectToPage("./Index");
        }
    }
}
