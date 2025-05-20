using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSystem.Models
{
    // This class represents a reservation in the reservation system.
    // It contains properties for the reservation's ID, instructor ID, classroom ID, term ID,
    // start/end times, status, and related navigation properties.
    public class Reservation
    {
        // Unique identifier for the reservation
        public int Id { get; set; }

        // ID of the instructor who made the reservation (required)
        [Required]
        public string InstructorId { get; set; } = string.Empty;

        // ID of the classroom being reserved
        public int ClassroomId { get; set; }
        // ID of the academic term for the reservation
        public int TermId { get; set; }

        // Start time of the reservation
        public DateTime StartTime { get; set; }
        // End time of the reservation
        public DateTime EndTime { get; set; }

        // Status of the reservation ("Pending", "Approved", "Rejected", "Cancelled")
        public string Status { get; set; } = "Pending";
        // Indicates if the reservation is cancelled
        public bool IsCancelled { get; set; } = false;
        // Date and time when the reservation was created (default: now, UTC)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for the related classroom
        public Classroom Classroom { get; set; }
        // Navigation property for the related term
        public Term Term { get; set; }
        // Collection of feedbacks associated with this reservation
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        // Collection of request logs for this reservation
        public ICollection<ReservationRequestLog> RequestLogs { get; set; } = new List<ReservationRequestLog>();

        // Navigation property for the instructor who made the reservation
        public ApplicationUser Instructor { get; set; } = null!;
    }
}
