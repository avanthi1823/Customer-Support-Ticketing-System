using CSTS.Models;
using Microsoft.EntityFrameworkCore;

namespace CSTS.Data
{
    public class CstsDbContext : DbContext
    {
        public CstsDbContext(DbContextOptions<CstsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(u => u.Role)
                      .IsRequired();

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);
            });


            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.TicketId);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Description)
                      .IsRequired()
                      .HasMaxLength(1500);

                entity.Property(t => t.Priority)
                      .IsRequired();

                entity.Property(t => t.Status)
                      .IsRequired();

                entity.Property(t => t.CreatedAt)
                      .IsRequired();

                

                entity.HasOne(t => t.CreatedBy)
                      .WithMany(u => u.CreatedTickets)
                      .HasForeignKey(t => t.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedTo)
                      .WithMany(u => u.AssignedTickets)
                      .HasForeignKey(t => t.AssignedToId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.CommentId);

                entity.Property(c => c.Message)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(c => c.CreatedDate)
                      .IsRequired();

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Ticket)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
