namespace AppGastos.Api.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? MonthlyBudget { get; set; }

    public List<Expense> Expenses { get; set; } = new();
}
