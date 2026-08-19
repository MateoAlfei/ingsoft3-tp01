using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppGastos.Api.Services;

public class CategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryResponse>> GetAllAsync(Guid userId)
    {
        return await _db.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.MonthlyBudget, c.Expenses.Any()))
            .ToListAsync();
    }

    public async Task<CategoryResponse> CreateAsync(Guid userId, CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("El nombre de la categoría es requerido.");

        if (request.MonthlyBudget is < 0)
            throw new ValidationException("El presupuesto mensual no puede ser negativo.");

        var category = new Category
        {
            UserId = userId,
            Name = request.Name.Trim(),
            MonthlyBudget = request.MonthlyBudget
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CategoryResponse(category.Id, category.Name, category.MonthlyBudget, false);
    }

    public async Task DeleteAsync(Guid userId, Guid categoryId)
    {
        var category = await _db.Categories
            .Include(c => c.Expenses)
            .SingleOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

        if (category is null)
            throw new NotFoundException("Categoría no encontrada.");

        if (category.Expenses.Count > 0)
            throw new ConflictException("No se puede eliminar una categoría que tiene gastos asociados.");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }
}
