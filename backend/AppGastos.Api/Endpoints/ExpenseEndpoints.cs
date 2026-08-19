using System.Security.Claims;
using AppGastos.Api.Common;
using AppGastos.Api.Dtos;
using AppGastos.Api.Services;

namespace AppGastos.Api.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/expenses").RequireAuthorization();

        group.MapGet("/", async (int? month, int? year, ClaimsPrincipal user, ExpenseService service) =>
        {
            var userId = user.GetUserId();
            return Results.Ok(await service.GetAllAsync(userId, month, year));
        });

        group.MapPost("/", async (CreateExpenseRequest request, ClaimsPrincipal user, ExpenseService service) =>
        {
            try
            {
                var userId = user.GetUserId();
                var result = await service.CreateAsync(userId, request);
                return Results.Created($"/api/expenses/{result.Id}", result);
            }
            catch (AppException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, ExpenseService service) =>
        {
            try
            {
                var userId = user.GetUserId();
                await service.DeleteAsync(userId, id);
                return Results.NoContent();
            }
            catch (AppException ex)
            {
                return ex.ToProblem();
            }
        });
    }
}
