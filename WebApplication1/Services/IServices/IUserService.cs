using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices;

public interface IUserService
{
    bool IsUniqueUser(string email);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<LoginResponseDTO> Register(RegistrationRequestDTO registrationRequestDTO);
}
