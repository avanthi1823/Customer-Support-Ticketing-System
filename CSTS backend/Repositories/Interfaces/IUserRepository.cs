using CSTS.DTOs.UserDTO;
using CSTS.Models;

namespace CSTS.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
       
        Task<UserResponseDTO?> RegisterAsync(RegisterUserDTO dto);
        Task<UserResponseDTO?> LoginAsync(LoginUserDTO dto);
        Task<UserResponseDTO?> GetByEmailAsync(string email);


        Task<IEnumerable<UserResponseDTO>> GetAllUsersMappedAsync();
        Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(UserRole role);

    
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
        Task<bool> DeleteUserByRoleAsync(int userId, UserRole expectedRole);
    }
}
