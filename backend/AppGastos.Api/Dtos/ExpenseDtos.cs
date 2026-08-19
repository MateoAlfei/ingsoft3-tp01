namespace AppGastos.Api.Dtos;

public record CreateExpenseRequest(Guid CategoryId, decimal Amount, string? Description, DateOnly Date);
public record ExpenseResponse(Guid Id, Guid CategoryId, string CategoryName, decimal Amount, string? Description, DateOnly Date);
