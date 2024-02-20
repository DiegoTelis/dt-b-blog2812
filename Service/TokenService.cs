using Blog.Extensions;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Service;

public class TokenService
{
    public string GenereteToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.JWTKey);
        var claims = user.GetClaims();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Subject = new ClaimsIdentity( new Claim[]
            //{
            //    new Claim(ClaimTypes.Name, "diegot"),   // User.Identity.Name
            //    new Claim(ClaimTypes.Role, "user"),    // User.IsInRole
            //    new Claim(ClaimTypes.Role, "admin"),    // User.IsInRole
            //    new Claim("fruta","banana")
            //}),
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
               
    }
}
