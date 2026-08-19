using AppGastos.Api.Common;
using AppGastos.Api.Dtos;
using AppGastos.Api.Services;

namespace AppGastos.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, AuthService authService) =>
        {
            try
            {
                var result = await authService.RegisterAsync(request);
                return Results.Ok(result);
            }
            catch (AppException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapPost("/login", async (LoginRequest request, AuthService authService) =>
        {
            try
            {
                var result = await authService.LoginAsync(request);
                return Results.Ok(result);
            }
            catch (AppException ex)
            {
                return ex.ToProblem();
            }
        });
    }
}
