using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Logica;            
using Microsoft.EntityFrameworkCore;

namespace AppGastos.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid userId, int month, int year)
    {
        // 1. Trae de la base las categorías del usuario y cuánto gastó en cada una ese mes (NO cambia)
        var categories = await _db.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.MonthlyBudget,
                Spent = c.Expenses
                    .Where(x => x.Date.Month == month && x.Date.Year == year)
                    .Sum(x => (decimal?)x.Amount) ?? 0m
            })
            .ToListAsync();

        // 2. Arma el resumen de cada categoría (ACÁ está el cambio)
        var summaries = categories
            .Select(c => new CategorySummary(
                c.Id,
                c.Name,
                c.MonthlyBudget,
                c.Spent,
                Presupuesto.Restante(c.MonthlyBudget, c.Spent)))   // ← antes: c.MonthlyBudget is null ? null : c.MonthlyBudget - c.Spent
            .ToList();

        return new DashboardSummaryResponse(month, year, summaries.Sum(s => s.Spent), summaries);
    }
}