using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models
{
    // This class represents an academic term in the reservation system.
    // It contains properties for the term's ID, name, start/end dates, and related reservations.
    public class Term
    {
        // Unique identifier for the term
        public int Id { get; set; }

        // Name of the term (required)
        [Required]
        public string Name { get; set; } = string.Empty;

        // Start date of the term
        public DateTime StartDate { get; set; }
        // End date of the term
        public DateTime EndDate { get; set; }

        // Collection of reservations associated with this term
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
