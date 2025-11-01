using System;
using System.ComponentModel.DataAnnotations;

namespace CSTS.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(1000)]
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}
