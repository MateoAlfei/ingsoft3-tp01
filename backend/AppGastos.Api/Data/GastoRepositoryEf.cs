using AppGastos.Api.Dtos;
using AppGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppGastos.Api.Data;

// La implementación REAL del contrato: habla con PostgreSQL usando Entity Framework.
// Las consultas son las MISMAS que antes estaban dentro de ExpenseService: solo se mudaron.
public class GastoRepositoryEf : IGastoRepository
{
    private readonly AppDbContext _db;

    public GastoRepositoryEf(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ExpenseResponse>> ListarAsync(Guid userId, int? month, int? year)
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

    public Task<Category?> BuscarCategoriaAsync(Guid userId, Guid categoryId)
        => _db.Categories.SingleOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

    public async Task AgregarAsync(Expense expense)
    {
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
    }

    public Task<Expense?> BuscarAsync(Guid userId, Guid expenseId)
        => _db.Expenses.SingleOrDefaultAsync(x => x.Id == expenseId && x.UserId == userId);

    public async Task EliminarAsync(Expense expense)
    {
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
    }
}