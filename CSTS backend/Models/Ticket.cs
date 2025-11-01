using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSTS.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(1500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.New;

        
        [Required]
        public int CreatedById { get; set; }

        public int? AssignedToId { get; set; }

     
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Navigation properties
        public User CreatedBy { get; set; } = null!;
        public User? AssignedTo { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum TicketPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum TicketStatus
    {
        New = 1,
        Assigned = 2,
        InProgress = 3,
        Resolved = 4,
        Closed = 5
    }
}
