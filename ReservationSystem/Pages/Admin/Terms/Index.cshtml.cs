using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Terms
{
    // PageModel for listing academic terms with search and pagination
    public class IndexModel : PageModel
    {
        // Database context for accessing terms
        private readonly ReservationsDbContext _context;

        // Constructor to inject the database context
        public IndexModel(ReservationsDbContext context)
        {
            _context = context;
        }

        // List of terms to display in the view
        public IList<Term> TermList { get; set; } = default!;
        // Search term entered by the user
        public string? SearchTerm { get; set; }
        // Current page number for pagination
        public int CurrentPage { get; set; }
        // Total number of pages for pagination
        public int TotalPages { get; set; }

        // Number of terms to display per page
        private const int PageSize = 10;

        // Handles GET requests to display the term list with search and pagination
        public async Task OnGetAsync(string? search, int page = 1)
        {
            SearchTerm = search;
            CurrentPage = page;

            // Query all terms
            var query = _context.Terms.AsQueryable();

            // Filter terms by search term if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Name.Contains(search));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated term list, ordered by start date descending
            TermList = await query
                .OrderByDescending(t => t.StartDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
