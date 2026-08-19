import { useEffect, useState, type FormEvent } from "react";
import { ApiError, categoriesApi } from "../api/client";
import type { Category } from "../api/types";

export function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [name, setName] = useState("");
  const [budget, setBudget] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    const data = await categoriesApi.getAll();
    setCategories(data);
    setLoading(false);
  }

  useEffect(() => {
    load();
  }, []);

  // Regla de frontend: no se puede enviar el formulario con datos inválidos.
  const isValid = name.trim().length > 0 && (budget.trim() === "" || Number(budget) >= 0);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!isValid) return;
    setError(null);
    try {
      await categoriesApi.create({
        name: name.trim(),
        monthlyBudget: budget.trim() === "" ? null : Number(budget),
      });
      setName("");
      setBudget("");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo crear la categoría.");
    }
  }

  async function handleDelete(id: string) {
    setError(null);
    try {
      await categoriesApi.remove(id);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo eliminar la categoría.");
    }
  }

  return (
    <div>
      <h1>Categorías</h1>

      <form className="card inline-form" onSubmit={handleSubmit}>
        <label>
          Nombre
          <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Comida" />
        </label>
        <label>
          Presupuesto mensual
          <input
            type="number"
            min={0}
            step="0.01"
            value={budget}
            onChange={(e) => setBudget(e.target.value)}
            placeholder="Opcional"
          />
        </label>
        <button type="submit" disabled={!isValid}>
          Agregar
        </button>
      </form>

      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Cargando...</p>
      ) : categories.length === 0 ? (
        <p>Todavía no creaste ninguna categoría.</p>
      ) : (
        <table className="table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Presupuesto</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {categories.map((c) => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.monthlyBudget != null ? `$${c.monthlyBudget.toFixed(2)}` : "—"}</td>
                <td>
                  {/* Regla de frontend: deshabilitado si la categoría tiene gastos asociados. */}
                  <button
                    type="button"
                    disabled={c.hasExpenses}
                    title={c.hasExpenses ? "No se puede eliminar: tiene gastos asociados" : undefined}
                    onClick={() => handleDelete(c.id)}
                  >
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
