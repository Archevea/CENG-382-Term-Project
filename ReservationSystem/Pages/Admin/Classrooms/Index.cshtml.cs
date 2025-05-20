using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Pages_Admin_Classrooms
{
    // PageModel for listing classrooms with search and pagination
    public class IndexModel : PageModel
    {
        // Database context for accessing classrooms and feedbacks
        private readonly ReservationsDbContext _context;

        // Constructor to inject the database context
        public IndexModel(ReservationsDbContext context)
        {
            _context = context;
        }

        // List of classrooms to display in the view
        public List<ClassroomViewModel> ClassroomList { get; set; } = new();
        // Search term entered by the user
        public string? SearchTerm { get; set; }
        // Current page number for pagination
        public int CurrentPage { get; set; }
        // Total number of pages for pagination
        public int TotalPages { get; set; }

        // Number of classrooms to display per page
        private const int PageSize = 10;

        // Handles GET requests to display the classroom list with search and pagination
        public async Task OnGetAsync(string? search, int page = 1)
        {
            SearchTerm = search;
            CurrentPage = page;

            // Query classrooms and include feedbacks for average rating calculation
            var query = _context.Classrooms
                .Include(c => c.Feedbacks)
                .AsQueryable();

            // Filter classrooms by search term if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Retrieve paginated classroom list and calculate average rating
            ClassroomList = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassroomViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Capacity = c.Capacity,
                    AverageRating = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Rating) : null
                })
                .ToListAsync();
        }

        // ViewModel for displaying classroom info in the list
        public class ClassroomViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public int Capacity { get; set; }
            public double? AverageRating { get; set; }
        }
    }
}
