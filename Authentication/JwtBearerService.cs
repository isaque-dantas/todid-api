using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoAPI.Models;

namespace TodoAPI.Authentication;

public static class JwtBearerService
{
    public static AuthResult GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettingsService.JwtSettings.Key);
        var tokenExpiration = DateTime.UtcNow.AddHours(2);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Expires = tokenExpiration,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResult(tokenHandler.WriteToken(token), (tokenExpiration - DateTime.UtcNow).TotalSeconds);
    }
}