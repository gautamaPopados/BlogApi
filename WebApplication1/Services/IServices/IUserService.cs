using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices;

public interface IUserService
{
    public bool IsUniqueUser(string email);
    public Task<TokenResponse> Login(LoginCredentials loginRequestDTO);
    public Task<TokenResponse> Register(UserRegistrationModel registrationRequestDTO);
    public Task Logout(string token);
    public Task Edit(string token, UserEditModel model);
    public Task<UserDto> GetProfile(string token);
}
