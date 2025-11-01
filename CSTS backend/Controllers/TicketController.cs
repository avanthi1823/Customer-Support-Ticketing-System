using CSTS.DTOs;
using CSTS.DTOs.TicketDTO;
using CSTS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSTS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;

        public TicketController(ITicketRepository ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateTicketDTO dto)
        {
            var result = await _ticketRepo.CreateTicketAsync(dto);
            if (result == null)
                return BadRequest("Invalid user or data.");

            return Ok(result);
        }

        [Authorize(Roles = "Admin, Agent")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketRepo.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketRepo.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound("Ticket not found.");

            return Ok(ticket);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var tickets = await _ticketRepo.GetTicketsByUserAsync(userId);
            return Ok(tickets);
        }

        [HttpPut("{ticketId}/assign/{adminId}/to/{agentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(int ticketId, int adminId, int agentId)
        {
            var result = await _ticketRepo.AssignTicketAsync(ticketId, adminId, agentId);
            if (!result)
                return BadRequest("Assignment failed. Check roles or IDs.");

            return Ok("Ticket successfully assigned to agent.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDTO dto)
        {
            var result = await _ticketRepo.UpdateTicketAsync(id, dto);
            if (!result)
                return NotFound("Ticket not found.");

            return Ok("Ticket updated successfully.");
        }
        
        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var result = await _ticketRepo.ResolveTicketAsync(id);
            if (!result)
                return BadRequest("Ticket not found or already resolved/closed.");

            return Ok($"Ticket #{id} marked as Resolved.");
        }


        [HttpPut("{id}/close")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var result = await _ticketRepo.CloseTicketAsync(id);
            if (!result)
                return BadRequest("Ticket not found or not yet resolved.");

            return Ok($"Ticket #{id} marked as Closed.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ticketRepo.DeleteTicketAsync(id);
            if (!result)
                return BadRequest("Cannot delete ticket unless it is closed.");

            return Ok("Ticket deleted successfully.");
        }
    }
}
