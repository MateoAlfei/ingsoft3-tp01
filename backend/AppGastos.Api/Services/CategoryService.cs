using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Logica;           
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

    // Método 1: listar las categorías del usuario (NO cambia)
    public async Task<List<CategoryResponse>> GetAllAsync(Guid userId)
    {
        return await _db.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.MonthlyBudget, c.Expenses.Any()))
            .ToListAsync();
    }

    // Método 2: crear una categoría (ACÁ está el cambio)
    public async Task<CategoryResponse> CreateAsync(Guid userId, CreateCategoryRequest request)
    {
        // Antes había dos "if" y un .Trim(). Ahora la regla vive en CategoriaValidator.
        var nombre = CategoriaValidator.Validar(request.Name, request.MonthlyBudget);

        var category = new Category
        {
            UserId = userId,
            Name = nombre,                     // ← antes era request.Name.Trim()
            MonthlyBudget = request.MonthlyBudget
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CategoryResponse(category.Id, category.Name, category.MonthlyBudget, false);
    }

    // Método 3: borrar una categoría (NO cambia)
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