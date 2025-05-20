using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models
{
    // This class represents a classroom in the reservation system.
    // It contains properties for the classroom's ID, name, capacity, and related reservations and feedback.
    public class Classroom
    {
        // Unique identifier for the classroom
        public int Id { get; set; }

        // Name of the classroom (required)
        [Required]
        public string Name { get; set; } = string.Empty;

        // Capacity of the classroom
        public int Capacity { get; set; }

        // Collection of reservations associated with this classroom
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        // Collection of feedback entries associated with this classroom
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
