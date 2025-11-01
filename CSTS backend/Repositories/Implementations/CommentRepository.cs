using CSTS.Data;
using CSTS.DTOs;
using CSTS.DTOs.CommentDTO;
using CSTS.Models;
using CSTS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSTS.Repositories.Implementations
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(CstsDbContext context) : base(context) { }

        public async Task<CommentResponseDTO> AddCommentAsync(CreateCommentDTO dto)
        {
            var comment = new Comment
            {
                TicketId = dto.TicketId,
                UserId = dto.UserId,
                Message = dto.Message,
                CreatedDate = DateTime.UtcNow
            };

            await AddAsync(comment);

            var user = await _context.Users.FindAsync(dto.UserId);

            return new CommentResponseDTO
            {
                CommentId = comment.CommentId,
                Message = comment.Message,
                UserName = user?.Name ?? "Unknown",
                CreatedDate = comment.CreatedDate
            };
        }

        public async Task<IEnumerable<CommentResponseDTO>> GetCommentsByTicketIdMappedAsync(int ticketId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return comments.Select(c => new CommentResponseDTO
            {
                CommentId = c.CommentId,
                Message = c.Message,
                UserName = c.User?.Name ?? "Unknown",
                CreatedDate = c.CreatedDate
            });
        }
    }
}


