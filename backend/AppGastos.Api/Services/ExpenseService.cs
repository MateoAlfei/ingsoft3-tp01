using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Logica;
using AppGastos.Api.Models;

namespace AppGastos.Api.Services;

public class ExpenseService
{
    // ANTES: private readonly AppDbContext _db;   (dependía de la base concreta)
    // AHORA: depende del contrato. No sabe si del otro lado hay Postgres o un impostor.
    private readonly IGastoRepository _repo;

    // La dependencia ENTRA desde afuera, por el constructor.
    // La app real le pasa GastoRepositoryEf; el test le pasa un Mock.
    public ExpenseService(IGastoRepository repo)
    {
        _repo = repo;
    }

    public Task<List<ExpenseResponse>> GetAllAsync(Guid userId, int? month, int? year)
        => _repo.ListarAsync(userId, month, year);

    public async Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request)
    {
        // 1. Validar (IGUAL que antes: la regla sigue acá adentro)
        GastoValidator.Validar(request.Amount, request.Date, DateOnly.FromDateTime(DateTime.UtcNow));

        // 2. Buscar la categoría (antes: _db.Categories.SingleOrDefaultAsync(...))
        var category = await _repo.BuscarCategoriaAsync(userId, request.CategoryId);

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

        // 3. Guardar (antes: _db.Expenses.Add(...) + SaveChangesAsync())
        await _repo.AgregarAsync(expense);

        return new ExpenseResponse(expense.Id, category.Id, category.Name, expense.Amount, expense.Description, expense.Date);
    }

    public async Task DeleteAsync(Guid userId, Guid expenseId)
    {
        var expense = await _repo.BuscarAsync(userId, expenseId);

        if (expense is null)
            throw new NotFoundException("Gasto no encontrado.");

        await _repo.EliminarAsync(expense);
    }
}