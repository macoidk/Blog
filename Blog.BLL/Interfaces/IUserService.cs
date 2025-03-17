using BlogSystem.BLL.DTO;

namespace BlogSystem.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> RegisterAsync(string username, string email, string password);
        Task<UserDto> AuthenticateAsync(string username, string password);
        Task UpdateAsync(UserDto userDto, string password = null);
        Task DeleteAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}