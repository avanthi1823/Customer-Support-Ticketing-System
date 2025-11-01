using CSTS.Data;
using CSTS.DTOs;
using CSTS.DTOs.TicketDTO;
using CSTS.Models;
using CSTS.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSTS.Tests.Repositories
{
    [TestFixture]
    public class TicketRepositoryTests
    {
        private CstsDbContext _context;
        private TicketRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CstsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CstsDbContext(options);

            
            _context.Users.AddRange(
                new User { UserId = 1, Name = "Customer1", Email = "cust@test.com", PasswordHash = "123", Role = UserRole.Customer },
                new User { UserId = 2, Name = "Admin1", Email = "admin@test.com", PasswordHash = "123", Role = UserRole.Admin },
                new User { UserId = 3, Name = "Agent1", Email = "agent@test.com", PasswordHash = "123", Role = UserRole.Agent }
            );
            _context.SaveChanges();

            _repository = new TicketRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

       
        [Test]
        public async Task CreateTicketAsync_ShouldCreateTicket_ForCustomer()
        {
            var dto = new CreateTicketDTO
            {
                Title = "System not working",
                Description = "Unable to open dashboard",
                CreatedById = 1
            };

            var result = await _repository.CreateTicketAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Title, result.Title);
            Assert.AreEqual(TicketStatus.New, result.Status);
            Assert.AreEqual("Customer1", result.CreatedBy);
        }

     
        [Test]
        public async Task CreateTicketAsync_ShouldReturnNull_IfInvalidUser()
        {
            var dto = new CreateTicketDTO
            {
                Title = "Test",
                Description = "Invalid user",
                CreatedById = 999
            };

            var result = await _repository.CreateTicketAsync(dto);

            Assert.IsNull(result);
        }

        [Test]
        public async Task AssignTicketAsync_ShouldAssignToAgent_WhenAdminAssigns()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Sample Ticket",
                Description = "To assign",
                CreatedById = 1,
                Status = TicketStatus.New
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var success = await _repository.AssignTicketAsync(1, 2, 3);

            Assert.IsTrue(success);
            var updated = await _context.Tickets.FindAsync(1);
            Assert.AreEqual(3, updated.AssignedToId);
            Assert.AreEqual(TicketStatus.Assigned, updated.Status);
        }

      
        [Test]
        public async Task ResolveTicketAsync_ShouldUpdateStatusToResolved()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Resolve Ticket",
                Description = "To resolve",
                CreatedById = 1,
                Status = TicketStatus.InProgress
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var success = await _repository.ResolveTicketAsync(1);

            Assert.IsTrue(success);
            var updated = await _context.Tickets.FindAsync(1);
            Assert.AreEqual(TicketStatus.Resolved, updated.Status);
            Assert.NotNull(updated.ResolvedAt);
        }

        [Test]
        public async Task CloseTicketAsync_ShouldCloseResolvedTicket()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Close Ticket",
                Description = "To close",
                CreatedById = 1,
                Status = TicketStatus.Resolved,
                ResolvedAt = DateTime.UtcNow
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var success = await _repository.CloseTicketAsync(1);

            Assert.IsTrue(success);
            var updated = await _context.Tickets.FindAsync(1);
            Assert.AreEqual(TicketStatus.Closed, updated.Status);
            Assert.NotNull(updated.ClosedAt);
        }

       [Test]
        public async Task UpdateTicketAsync_ShouldUpdateDetailsAndSetResolvedDate()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Old Title",
                Description = "Old Desc",
                CreatedById = 1,
                Status = TicketStatus.InProgress
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var dto = new UpdateTicketDTO
            {
                Title = "New Title",
                Description = "Updated Desc",
                Status = (int)TicketStatus.Resolved
            };

            var success = await _repository.UpdateTicketAsync(1, dto);

            Assert.IsTrue(success);
            var updated = await _context.Tickets.FindAsync(1);
            Assert.AreEqual("New Title", updated.Title);
            Assert.AreEqual(TicketStatus.Resolved, updated.Status);
            Assert.NotNull(updated.ResolvedAt);
        }
        [Test]
        public async Task DeleteTicketAsync_ShouldDeleteOnlyClosedTicket()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Delete Me",
                Description = "For test",
                CreatedById = 1,
                Status = TicketStatus.Closed
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var success = await _repository.DeleteTicketAsync(1);

            Assert.IsTrue(success);
            Assert.AreEqual(0, _context.Tickets.Count());
        }

        [Test]
        public async Task DeleteTicketAsync_ShouldFailIfNotClosed()
        {
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Active Ticket",
                Description = "Still open",
                CreatedById = 1,
                Status = TicketStatus.InProgress
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            var success = await _repository.DeleteTicketAsync(1);

            Assert.IsFalse(success);
            Assert.AreEqual(1, _context.Tickets.Count());
        }
    }
}
