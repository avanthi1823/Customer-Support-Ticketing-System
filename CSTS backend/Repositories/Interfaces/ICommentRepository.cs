using CSTS.DTOs;
using CSTS.DTOs.CommentDTO;
using CSTS.Models;

namespace CSTS.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<CommentResponseDTO> AddCommentAsync(CreateCommentDTO dto);
        Task<IEnumerable<CommentResponseDTO>> GetCommentsByTicketIdMappedAsync(int ticketId);
    }
}
