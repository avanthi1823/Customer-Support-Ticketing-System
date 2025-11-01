using CSTS.DTOs.UserDTO;
using CSTS.Models;
using CSTS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSTS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

       
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsersMappedAsync();
            return Ok(users);
        }

        
        [HttpGet("agents")]
        public async Task<IActionResult> GetAllAgents()
        {
            var users = await _userRepo.GetAllUsersMappedAsync();
            var agents = users.Where(u => u.Role == UserRole.Agent.ToString());
            return Ok(agents);
        }

      
        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var users = await _userRepo.GetAllUsersMappedAsync();
            var admins = users.Where(u => u.Role == UserRole.Admin.ToString());
            return Ok(admins);
        }

       
        [HttpPost("add-agent")]
        public async Task<IActionResult> AddAgent([FromBody] RegisterUserDTO dto)
        {
            dto.Role = "Agent";
            var result = await _userRepo.RegisterAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Agent registration failed (duplicate email or invalid data)" });

            return Ok(new { message = "Agent added successfully", data = result });
        }

     
        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdmin([FromBody] RegisterUserDTO dto)
        {
            dto.Role = "Admin";
            var result = await _userRepo.RegisterAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Admin registration failed (duplicate email or invalid data)" });

            return Ok(new { message = "Admin added successfully", data = result });
        }

       
        [HttpDelete("remove-agent/{id}")]
        public async Task<IActionResult> RemoveAgent(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Agent not found" });

            if (user.Role != UserRole.Agent)
                return BadRequest(new { message = "User is not an agent" });

            await _userRepo.DeleteAsync(id);
            return Ok(new { message = "Agent removed successfully" });
        }

        
        [HttpDelete("remove-customer/{id}")]
        public async Task<IActionResult> RemoveCustomer(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Customer not found" });

            if (user.Role != UserRole.Customer)
                return BadRequest(new { message = "User is not a customer" });

            await _userRepo.DeleteAsync(id);
            return Ok(new { message = "Customer removed successfully" });
        }

       
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var result = await _userRepo.UpdateUserStatusAsync(id, true);
            if (!result)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User activated successfully" });
        }

       
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var result = await _userRepo.UpdateUserStatusAsync(id, false);
            if (!result)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deactivated successfully" });
        }
    }
}
