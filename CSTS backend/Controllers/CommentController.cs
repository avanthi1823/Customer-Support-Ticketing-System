
using CSTS.DTOs.CommentDTO;
using CSTS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSTS.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Agent,Customer")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid comment data.");

            if (dto.TicketId <= 0)
                return BadRequest("TicketId is required.");

            if (dto.UserId <= 0)
                return BadRequest("UserId is required.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Comment message cannot be empty.");

            Console.WriteLine($"📥 AddComment received: TicketId={dto.TicketId}, UserId={dto.UserId}, Message='{dto.Message}'");

            try
            {
                var comment = await _commentRepository.AddCommentAsync(dto);
                if (comment == null)
                {
                    Console.WriteLine("❌ AddCommentAsync returned null!");
                    return StatusCode(500, "Failed to create comment.");
                }

                Console.WriteLine("✅ Comment saved successfully!");
                return Ok(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Exception in AddComment: {ex.Message}");
                return StatusCode(500, "Internal server error while adding comment.");
            }
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetCommentsByTicket(int ticketId)
        {
            if (ticketId <= 0)
                return BadRequest("Invalid ticket ID.");

            try
            {
                var comments = await _commentRepository.GetCommentsByTicketIdMappedAsync(ticketId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Exception in GetCommentsByTicket: {ex.Message}");
                return StatusCode(500, "Error retrieving comments.");
            }
        }
    }
}
