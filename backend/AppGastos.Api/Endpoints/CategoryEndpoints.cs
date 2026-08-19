using System.Security.Claims;
using AppGastos.Api.Common;
using AppGastos.Api.Dtos;
using AppGastos.Api.Services;

namespace AppGastos.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, CategoryService service) =>
        {
            var userId = user.GetUserId();
            return Results.Ok(await service.GetAllAsync(userId));
        });

        group.MapPost("/", async (CreateCategoryRequest request, ClaimsPrincipal user, CategoryService service) =>
        {
            try
            {
                var userId = user.GetUserId();
                var result = await service.CreateAsync(userId, request);
                return Results.Created($"/api/categories/{result.Id}", result);
            }
            catch (AppException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, CategoryService service) =>
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
