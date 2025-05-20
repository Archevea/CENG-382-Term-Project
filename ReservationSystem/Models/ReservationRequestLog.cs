namespace ReservationSystem.Models
{
    // This class represents a log entry for reservation requests in the reservation system.
    // It contains properties for the reservation ID, user who made the request, request type, time of request, and any notes.
    public class ReservationRequestLog
    {
        // Unique identifier for the log entry
        public int Id { get; set; }

        // ID of the reservation associated with this log entry
        public int ReservationId { get; set; }

        // ID of the user who made the request
        public string RequestedById { get; set; } = string.Empty;

        // Type of request (e.g., Create, Update, Cancel, Approve, Reject)
        public string RequestType { get; set; } = "Create";

        // Date and time when the request was made (default: now, UTC)
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;

        // Optional notes about the request
        public string? Notes { get; set; }

        // Navigation property for the related reservation
        public Reservation Reservation { get; set; }
    }
}
