using CSTS.DTOs;
using CSTS.DTOs.TicketDTO;
using CSTS.Models;

namespace CSTS.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<TicketResponseDTO?> CreateTicketAsync(CreateTicketDTO dto);
        Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync();
        Task<TicketResponseDTO?> GetTicketByIdAsync(int id);
        Task<bool> UpdateTicketAsync(int id, UpdateTicketDTO dto);
        Task<bool> DeleteTicketAsync(int id);
        Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserAsync(int userId);
        Task<bool> AssignTicketAsync(int ticketId, int adminId, int agentId);
        Task<bool> ResolveTicketAsync(int ticketId);
        Task<bool> CloseTicketAsync(int ticketId);
    }
}
