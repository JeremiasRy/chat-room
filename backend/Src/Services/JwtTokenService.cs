using backend.Src.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Src.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(ChatUser user)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.Ticks.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        };

        string secret = _configuration["Jwt:Secret"];
        var signinkey = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.Now.AddHours(_configuration.GetValue<int>("Jwt:ExpiresInHours"));
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiration,
            signingCredentials: signinkey
            );

        var writer = new JwtSecurityTokenHandler();

        return writer.WriteToken(token);
    }

    public bool CanReadToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.CanReadToken(token);
    }
}
