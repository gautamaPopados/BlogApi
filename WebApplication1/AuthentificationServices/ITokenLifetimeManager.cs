using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.AuthentificationServices
{
    public interface ITokenLifetimeManager
    {
        public bool ValidateTokenLifetime(DateTime? notBefore,
                                           DateTime? expires,
                                           SecurityToken securityToken,
                                           TokenValidationParameters validationParameters);
        public void SignOut(SecurityToken securityToken);
    }
}