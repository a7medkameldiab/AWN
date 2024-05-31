using AWN.Dtos.UserDto;
using AWN.Models;

namespace AWN.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterUserDto model);
        Task<AuthModel> LogInAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
