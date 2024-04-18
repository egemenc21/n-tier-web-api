using Server.Model.Models;

namespace Server.Business.Token;

public interface ITokenService
{
    string GenerateToken(AppUser user);
}