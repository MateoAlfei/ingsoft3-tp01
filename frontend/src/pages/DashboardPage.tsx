import { useEffect, useState } from "react";
import { dashboardApi } from "../api/client";
import type { DashboardSummary } from "../api/types";

const MONTH_NAMES = [
  "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
  "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
];

export function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const now = new Date();
    dashboardApi.getSummary(now.getMonth() + 1, now.getFullYear()).then((data) => {
      setSummary(data);
      setLoading(false);
    });
  }, []);

  if (loading || !summary) return <p>Cargando...</p>;

  return (
    <div>
      <h1>
        Dashboard — {MONTH_NAMES[summary.month - 1]} {summary.year}
      </h1>
      <p className="total">Total gastado: ${summary.totalSpent.toFixed(2)}</p>

      {summary.categories.length === 0 ? (
        <p>Todavía no hay categorías cargadas.</p>
      ) : (
        <div className="cards-grid">
          {summary.categories.map((c) => {
            const pct = c.monthlyBudget ? Math.min(100, (c.spent / c.monthlyBudget) * 100) : null;
            return (
              <div className="card" key={c.categoryId}>
                <h2>{c.categoryName}</h2>
                <p>Gastado: ${c.spent.toFixed(2)}</p>
                {c.monthlyBudget != null ? (
                  <>
                    <p>Presupuesto: ${c.monthlyBudget.toFixed(2)}</p>
                    <p className={c.remaining != null && c.remaining < 0 ? "error" : ""}>
                      Saldo restante: ${c.remaining?.toFixed(2)}
                    </p>
                    <div className="progress">
                      <div className="progress-bar" style={{ width: `${pct}%` }} />
                    </div>
                  </>
                ) : (
                  <p>Sin presupuesto definido</p>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
