using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models
{
    // This class represents feedback submitted by an instructor for a classroom.
    // It contains properties for the feedback's ID, instructor, comment, rating, submission date, and related classroom.
    public class Feedback
    {
        // Unique identifier for the feedback entry
        public int Id { get; set; }

        // ID of the instructor who submitted the feedback (required)
        [Required]
        public string InstructorId { get; set; } = string.Empty;

        // Comment provided by the instructor (required)
        [Required]
        public string Comment { get; set; } = string.Empty;

        // Rating given by the instructor (1 to 5)
        [Range(1, 5)]
        public int? Rating { get; set; }

        // Date and time when the feedback was submitted (default: now, UTC)
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // ID of the classroom for which the feedback was submitted
        public int ClassroomId { get; set; }
        // Navigation property for the related classroom
        public Classroom Classroom { get; set; } = null!;
    }
}
