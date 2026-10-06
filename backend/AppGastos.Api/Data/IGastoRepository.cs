using AppGastos.Api.Dtos;
using AppGastos.Api.Models;

namespace AppGastos.Api.Data;

// El "contrato" de lo que ExpenseService necesita de la base de datos.
// Dice QUÉ se puede pedir, no CÓMO se hace.
public interface IGastoRepository
{
    // Listar los gastos de un usuario (opcionalmente de un mes y año)
    Task<List<ExpenseResponse>> ListarAsync(Guid userId, int? month, int? year);

    // Buscar una categoría que sea de ese usuario (null si no existe o es de otro)
    Task<Category?> BuscarCategoriaAsync(Guid userId, Guid categoryId);

    // Guardar un gasto nuevo
    Task AgregarAsync(Expense expense);

    // Buscar un gasto del usuario (null si no existe o es de otro)
    Task<Expense?> BuscarAsync(Guid userId, Guid expenseId);

    // Borrar un gasto
    Task EliminarAsync(Expense expense);
}