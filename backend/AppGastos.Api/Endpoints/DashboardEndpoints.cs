using System.Security.Claims;
using AppGastos.Api.Common;
using AppGastos.Api.Services;

namespace AppGastos.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard").RequireAuthorization();

        group.MapGet("/summary", async (int? month, int? year, ClaimsPrincipal user, DashboardService service) =>
        {
            var userId = user.GetUserId();
            var now = DateTime.UtcNow;
            var result = await service.GetSummaryAsync(userId, month ?? now.Month, year ?? now.Year);
            return Results.Ok(result);
        });
    }
}
