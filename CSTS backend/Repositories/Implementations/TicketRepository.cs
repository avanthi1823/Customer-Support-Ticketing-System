using CSTS.Data;
using CSTS.DTOs;
using CSTS.DTOs.TicketDTO;
using CSTS.Models;
using CSTS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSTS.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly CstsDbContext _context;

        public TicketRepository(CstsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TicketResponseDTO?> CreateTicketAsync(CreateTicketDTO dto)
        {
            var creator = await _context.Users.FindAsync(dto.CreatedById);
            if (creator == null || creator.Role != UserRole.Customer)
                return null;

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = TicketPriority.Low,
                Status = TicketStatus.New,
                CreatedById = dto.CreatedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            return new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedBy = creator.Name,
                AssignedTo = null,
                CreatedAt = ticket.CreatedAt
            };
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                
                .Select(t => new TicketResponseDTO
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedBy = t.CreatedBy.Name,
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : "Unassigned",
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TicketResponseDTO?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return null;

            return new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedBy = ticket.CreatedBy.Name,
                AssignedTo = ticket.AssignedTo?.Name ?? "Unassigned",
                CreatedAt = ticket.CreatedAt
            };
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.CreatedById == userId)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Select(t => new TicketResponseDTO
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedBy = t.CreatedBy.Name,
                    AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : "Unassigned",
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> AssignTicketAsync(int ticketId, int adminId, int agentId)
        {
            var admin = await _context.Users.FindAsync(adminId);
            if (admin == null || admin.Role != UserRole.Admin)
                return false;

            var agent = await _context.Users.FindAsync(agentId);
            if (agent == null || agent.Role != UserRole.Agent)
                return false;

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null || ticket.AssignedToId != null)
                return false;

            ticket.AssignedToId = agentId;
            ticket.Status = TicketStatus.Assigned;
            ticket.AssignedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTicketAsync(int id, UpdateTicketDTO dto)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return false;

            
            if (!string.IsNullOrEmpty(dto.Title))
                ticket.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                ticket.Description = dto.Description;

            if (dto.Priority.HasValue)
                ticket.Priority = (TicketPriority)dto.Priority.Value;

            if (dto.Status.HasValue)
            {
                var newStatus = (TicketStatus)dto.Status.Value;

               
                switch (newStatus)
                {
                    case TicketStatus.Resolved:
                        ticket.ResolvedAt = DateTime.UtcNow;
                        break;

                    case TicketStatus.Closed:
                        
                        if (ticket.ResolvedAt == null)
                            ticket.ResolvedAt = DateTime.UtcNow;
                        ticket.ClosedAt = DateTime.UtcNow;
                        break;

                    case TicketStatus.InProgress:
                    case TicketStatus.Assigned:
                    case TicketStatus.New:
                        
                        ticket.ClosedAt = null;
                        ticket.ResolvedAt = null;
                        break;
                }

                ticket.Status = newStatus;
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }


       
        public async Task<bool> ResolveTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return false;

            if (ticket.Status == TicketStatus.Resolved || ticket.Status == TicketStatus.Closed)
                return false;

            ticket.Status = TicketStatus.Resolved;
            ticket.ResolvedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

     
        public async Task<bool> CloseTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return false;

          
            if (ticket.Status != TicketStatus.Resolved)
                return false;

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null || ticket.Status != TicketStatus.Closed)
                return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
