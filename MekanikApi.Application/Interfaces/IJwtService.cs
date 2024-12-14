using System.Security.Claims;

namespace MekanikApi.Application.Interfaces
{
    public interface IJwtService
    {
        ClaimsPrincipal GetTokenPrincipal(string accessToken);
    }
}