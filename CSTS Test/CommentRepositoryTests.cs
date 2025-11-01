using CSTS.Data;
using CSTS.DTOs.CommentDTO;
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
    public class CommentRepositoryTests
    {
        private CstsDbContext _context;
        private CommentRepository _repository;

        [SetUp]
        public void Setup()
        {
          
            var options = new DbContextOptionsBuilder<CstsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CstsDbContext(options);
            _repository = new CommentRepository(_context);

            var user = new User
            {
                UserId = 1,
                Name = "Avi",
                Email = "avi@test.com",
                PasswordHash = "hashed",
                Role = UserRole.Customer
            };

            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Login Issue",
                Description = "Cannot log in",
                Priority = TicketPriority.High,
                Status = TicketStatus.New,
                CreatedById = 1
            };

            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddCommentAsync_ShouldAddCommentSuccessfully()
        {
            
            var dto = new CreateCommentDTO
            {
                TicketId = 1,
                UserId = 1,
                Message = "We are checking this issue."
            };

            
            var result = await _repository.AddCommentAsync(dto);

            Assert.NotNull(result);
            Assert.AreEqual(dto.Message, result.Message);
            Assert.AreEqual("Avi", result.UserName);

            var savedComment = await _context.Comments.FirstOrDefaultAsync();
            Assert.NotNull(savedComment);
            Assert.AreEqual(dto.TicketId, savedComment.TicketId);
        }

        [Test]
        public async Task AddCommentAsync_ShouldReturnUnknownUser_WhenUserNotFound()
        {
          
            var dto = new CreateCommentDTO
            {
                TicketId = 1,
                UserId = 99,
                Message = "Checking invalid user case"
            };
  
            var result = await _repository.AddCommentAsync(dto);

           
            Assert.NotNull(result);
            Assert.AreEqual("Unknown", result.UserName);
            Assert.AreEqual(dto.Message, result.Message);
        }

        [Test]
        public async Task GetCommentsByTicketIdMappedAsync_ShouldReturnCommentsForTicket()
        {
            
            _context.Comments.AddRange(
                new Comment
                {
                    CommentId = 1,
                    TicketId = 1,
                    UserId = 1,
                    Message = "First Comment",
                    CreatedDate = DateTime.UtcNow
                },
                new Comment
                {
                    CommentId = 2,
                    TicketId = 1,
                    UserId = 1,
                    Message = "Second Comment",
                    CreatedDate = DateTime.UtcNow
                }
            );

            await _context.SaveChangesAsync();

        
            var result = await _repository.GetCommentsByTicketIdMappedAsync(1);

        
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.True(result.All(c => c.UserName == "Avi"));
        }

        [Test]
        public async Task GetCommentsByTicketIdMappedAsync_ShouldReturnEmpty_WhenNoCommentsExist()
        {
           
            var result = await _repository.GetCommentsByTicketIdMappedAsync(999); 

            
            Assert.NotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
