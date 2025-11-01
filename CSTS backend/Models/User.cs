using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSTS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; } 

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        
        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>(); 
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum UserRole
    {
        Admin = 1,
        Agent = 2,
        Customer = 3
    }
}
