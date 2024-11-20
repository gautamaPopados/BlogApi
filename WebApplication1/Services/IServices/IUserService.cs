using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices;

public interface IUserService
{
    bool IsUniqueUser(string email);
    Task<AuthorizationResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<AuthorizationResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO);

    bool IsTokenBlacklisted(string token);
    Task Logout(string token);

}
