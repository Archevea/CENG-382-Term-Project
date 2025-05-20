// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Models;

namespace ReservationSystem.Data
{
    // This class represents the database context for the reservation system.
    // It inherits from IdentityDbContext to include ASP.NET Core Identity support.
    public class ReservationsDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor to configure the DbContext with options
        public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for each entity in the database.
        // Represents the Terms table
        public DbSet<Term> Terms { get; set; }
        // Represents the Classrooms table
        public DbSet<Classroom> Classrooms { get; set; }
        // Represents the Reservations table
        public DbSet<Reservation> Reservations { get; set; }
        // Represents the Feedbacks table
        public DbSet<Feedback> Feedbacks { get; set; }
        // Represents the ReservationRequestLogs table
        public DbSet<ReservationRequestLog> ReservationRequestLogs { get; set; }
    }
}