namespace AppGastos.Api.Dtos;

public record CreateCategoryRequest(string Name, decimal? MonthlyBudget);
public record CategoryResponse(Guid Id, string Name, decimal? MonthlyBudget, bool HasExpenses);
