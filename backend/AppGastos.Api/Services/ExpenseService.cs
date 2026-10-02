using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Logica;
using AppGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppGastos.Api.Services;

public class ExpenseService
{
    private readonly AppDbContext _db;

    public ExpenseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ExpenseResponse>> GetAllAsync(Guid userId, int? month, int? year)
    {
        var query = _db.Expenses.Include(x => x.Category).Where(x => x.UserId == userId);

        if (month is not null && year is not null)
        {
            query = query.Where(x => x.Date.Month == month && x.Date.Year == year);
        }

        return await query
            .OrderByDescending(x => x.Date)
            .Select(x => new ExpenseResponse(x.Id, x.CategoryId, x.Category!.Name, x.Amount, x.Description, x.Date))
            .ToListAsync();
    }

        public async Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request)
    {
        GastoValidator.Validar(request.Amount, request.Date, DateOnly.FromDateTime(DateTime.UtcNow));

        var category = await _db.Categories
        
            .SingleOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);

        if (category is null)
            throw new NotFoundException("Categoría no encontrada.");

        var expense = new Expense
        {
            UserId = userId,
            CategoryId = category.Id,
            Amount = request.Amount,
            Description = request.Description?.Trim(),
            Date = request.Date
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return new ExpenseResponse(expense.Id, category.Id, category.Name, expense.Amount, expense.Description, expense.Date);
    }

    public async Task DeleteAsync(Guid userId, Guid expenseId)
    {
        var expense = await _db.Expenses.SingleOrDefaultAsync(x => x.Id == expenseId && x.UserId == userId);

        if (expense is null)
            throw new NotFoundException("Gasto no encontrado.");

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
    }
}
