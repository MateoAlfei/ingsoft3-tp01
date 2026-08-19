import { useEffect, useMemo, useState, type FormEvent } from "react";
import { ApiError, categoriesApi, expensesApi } from "../api/client";
import type { Category, Expense } from "../api/types";

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

export function ExpensesPage() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [categoryId, setCategoryId] = useState("");
  const [amount, setAmount] = useState("");
  const [description, setDescription] = useState("");
  const [date, setDate] = useState(todayIso());
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    const [expensesData, categoriesData] = await Promise.all([
      expensesApi.getAll(),
      categoriesApi.getAll(),
    ]);
    setExpenses(expensesData);
    setCategories(categoriesData);
    setLoading(false);
  }

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    if (!categoryId && categories.length > 0) {
      setCategoryId(categories[0].id);
    }
  }, [categories, categoryId]);

  // Regla de frontend: el total se recalcula solo, sin recargar la página.
  const total = useMemo(() => expenses.reduce((sum, e) => sum + e.amount, 0), [expenses]);

  // Regla de frontend: no se puede enviar el formulario con datos inválidos.
  const isValid = categoryId !== "" && Number(amount) > 0 && date !== "" && date <= todayIso();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!isValid) return;
    setError(null);
    try {
      const created = await expensesApi.create({
        categoryId,
        amount: Number(amount),
        description: description.trim() || null,
        date,
      });
      setExpenses((prev) => [created, ...prev]);
      setAmount("");
      setDescription("");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo cargar el gasto.");
    }
  }

  async function handleDelete(id: string) {
    setError(null);
    try {
      await expensesApi.remove(id);
      setExpenses((prev) => prev.filter((e) => e.id !== id));
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo eliminar el gasto.");
    }
  }

  return (
    <div>
      <h1>Gastos</h1>

      {!loading && categories.length === 0 && (
        <p>Primero creá una categoría para poder cargar gastos.</p>
      )}

      <form className="card inline-form" onSubmit={handleSubmit}>
        <label>
          Categoría
          <select value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
            <option value="" disabled>
              Elegir...
            </option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </label>
        <label>
          Monto
          <input
            type="number"
            min={0.01}
            step="0.01"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
          />
        </label>
        <label>
          Fecha
          <input type="date" max={todayIso()} value={date} onChange={(e) => setDate(e.target.value)} />
        </label>
        <label>
          Descripción
          <input
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Opcional"
          />
        </label>
        <button type="submit" disabled={!isValid}>
          Agregar gasto
        </button>
      </form>

      {error && <p className="error">{error}</p>}

      <p className="total">Total: ${total.toFixed(2)}</p>

      {loading ? (
        <p>Cargando...</p>
      ) : expenses.length === 0 ? (
        <p>Todavía no cargaste ningún gasto.</p>
      ) : (
        <table className="table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Categoría</th>
              <th>Descripción</th>
              <th>Monto</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {expenses.map((e) => (
              <tr key={e.id}>
                <td>{e.date}</td>
                <td>{e.categoryName}</td>
                <td>{e.description ?? "—"}</td>
                <td>${e.amount.toFixed(2)}</td>
                <td>
                  <button type="button" onClick={() => handleDelete(e.id)}>
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
