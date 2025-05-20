using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Classrooms
{
    // PageModel for the Create Classroom page
    public class CreateModel : PageModel
    {
        // Database context for accessing classrooms
        private readonly ReservationSystem.Data.ReservationsDbContext _context;

        // Constructor to inject the database context
        public CreateModel(ReservationSystem.Data.ReservationsDbContext context)
        {
            _context = context;
        }

        // Handles GET requests to display the form
        public IActionResult OnGet()
        {
            return Page();
        }

        // Property to bind form data to the Classroom model
        [BindProperty]
        public Classroom Classroom { get; set; } = default!;

        // Handles POST requests to create a new classroom
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form data is invalid, redisplay the form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new classroom to the database
            _context.Classrooms.Add(Classroom);
            await _context.SaveChangesAsync();

            // Redirect to the classroom list page after successful creation
            return RedirectToPage("./Index");
        }
    }
}
