using backend.Src.Models;

namespace backend.Src.Services;

public interface IJwtTokenService
{
    string CreateToken(ChatUser user);
}