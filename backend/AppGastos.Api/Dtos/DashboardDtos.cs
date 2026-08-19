namespace AppGastos.Api.Dtos;

public record CategorySummary(Guid CategoryId, string CategoryName, decimal? MonthlyBudget, decimal Spent, decimal? Remaining);
public record DashboardSummaryResponse(int Month, int Year, decimal TotalSpent, List<CategorySummary> Categories);
