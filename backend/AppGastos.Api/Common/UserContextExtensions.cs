using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppGastos.Api.Common;

public static class UserContextExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (sub is null || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedException("Token inválido.");

        return userId;
    }
}
