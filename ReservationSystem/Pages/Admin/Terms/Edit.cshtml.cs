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

namespace ReservationSystem.Pages_Admin_Terms
{
    // PageModel for editing an academic term
    public class EditModel : PageModel
    {
        // Database context for accessing terms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public EditModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Property to bind form data to the Term model
        [BindProperty]
        public Term Term { get; set; } = default!;

        // Handles GET requests to display the edit form for a term
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no ID is provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the term by ID
            var term =  await _context.Terms.FirstOrDefaultAsync(m => m.Id == id);
            if (term == null)
            {
                return NotFound();
            }
            Term = term;
            return Page();
        }

        // Handles POST requests to update the term
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form data is invalid, redisplay the form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Mark the term entity as modified
            _context.Attach(Term).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the term no longer exists, return 404
                if (!TermExists(Term.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to the term list page after successful edit
            return RedirectToPage("./Index");
        }

        // Helper method to check if a term exists by ID
        private bool TermExists(int id)
        {
            return _context.Terms.Any(e => e.Id == id);
        }
    }
}
