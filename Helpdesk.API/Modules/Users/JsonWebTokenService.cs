using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Helpdesk.API.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Helpdesk.API.Modules.Users
{
    public class JsonWebTokenService
    {
        private readonly ApplicationOptions _applicationOptions;

        public JsonWebTokenService(IOptions<ApplicationOptions> applicationOptions)
        {
            _applicationOptions = applicationOptions.Value;
        }

        public string GenerateTokenAsync(ClaimsIdentity identity)
        {
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_applicationOptions.JwtSecretKey)
                    ),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = new JwtSecurityTokenHandler().CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
