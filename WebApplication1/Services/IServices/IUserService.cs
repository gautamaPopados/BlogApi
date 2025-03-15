using WebApplication1.Data.DTO.Auth;
using WebApplication1.Data.DTO.User;

namespace WebApplication1.Services.IServices;

public interface IUserService
{
    public Task<bool> IsUniqueUser(string email);
    public Task<bool> IsUniquePhoneNumber(string phoneNumber);
    public Task<AuthResponseDTO> Login(LoginRequestDTO loginData);
    public Task<AuthResponseDTO> Register(RegistrationRequestDTO registrationData);
    public Task Logout(string accessToken);
    public Task ChangeProfileInfo(Guid userId, ChangeProfileRequestDTO newProfileInfo);
    public Task<ProfileResponseDTO> GetProfile(Guid userId);
}
