using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices;

public interface IUserService
{
    bool IsUniqueUser(string email);
    Task<TokenResponse> Login(LoginCredentials loginRequestDTO);
    Task<TokenResponse> Register(UserRegistrationModel registrationRequestDTO);
    Task Logout(string token);
    Task<UserDto> GetProfile(string token);
}
