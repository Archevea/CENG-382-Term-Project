using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Terms
{
    // PageModel for creating a new academic term
    public class CreateModel : PageModel
    {
        // Database context for accessing terms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public CreateModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Handles GET requests to display the create term form
        public IActionResult OnGet()
        {
            return Page();
        }

        // Property to bind form data to the Term model
        [BindProperty]
        public Term Term { get; set; } = default!;

        // Handles POST requests to create a new term
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form data is invalid, redisplay the form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new term to the database
            _context.Terms.Add(Term);
            await _context.SaveChangesAsync();

            // Redirect to the term list page after successful creation
            return RedirectToPage("./Index");
        }
    }
}
