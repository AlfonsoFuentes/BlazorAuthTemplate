using Shared.Dtos.Responses;
using System.Security.Claims;

namespace Server.Repositories
{
    public interface ITokenService
    {
        TokenResponse GetToken(IEnumerable<Claim> claim);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
