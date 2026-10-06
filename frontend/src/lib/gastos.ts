import type { DashboardSummary, Expense } from "../api/types";

// ── Regla 1: el total de los gastos ─────────────────────────────────────────
// Suma los montos de una lista de gastos. Una lista vacía da 0.
export function totalGastado(gastos: Pick<Expense, "amount">[]): number {
  return gastos.reduce((suma, g) => suma + g.amount, 0);
}

// ── Regla 2: si el formulario de gasto se puede enviar ──────────────────────
// Los datos tal como vienen del formulario (los inputs siempre dan texto).
export interface FormGasto {
  categoryId: string;
  amount: string;
  date: string; // formato "AAAA-MM-DD"
}

// "hoy" entra por parámetro (también "AAAA-MM-DD"), igual que en el backend:
// así el test no depende del reloj.
export function esGastoValido(form: FormGasto, hoy: string): boolean {
  return (
    form.categoryId !== "" &&     // eligió una categoría
    Number(form.amount) > 0 &&    // el monto es mayor a 0
    form.date !== "" &&           // puso una fecha
    form.date <= hoy              // y no es futura
  );
}

// ── Regla 3: el % de la barra de presupuesto ────────────────────────────────
// Sin presupuesto (o presupuesto 0) no hay porcentaje que mostrar → null.
// Con presupuesto, el % gastado, con TOPE en 100 (la barra no puede pasar del ancho total).
export function porcentajeUsado(gastado: number, presupuesto: number | null): number | null {
  if (!presupuesto) return null;
  return Math.min(100, (gastado / presupuesto) * 100);
}

// ── Regla 4: pedir el resumen del mes actual ────────────────────────────────
// "obtener" es la función de la API que trae el resumen. Entra por parámetro
// para que en el test podamos pasarle un impostor (mock) en lugar de la API real.
export type ObtenerResumen = (mes: number, anio: number) => Promise<DashboardSummary>;

export function resumenDelMes(obtener: ObtenerResumen, hoy: Date): Promise<DashboardSummary> {
  // getMonth() devuelve 0..11 (enero = 0), pero la API espera 1..12 (enero = 1)
  return obtener(hoy.getMonth() + 1, hoy.getFullYear());
}
export function proyeccionFinDeMes(gastado: number, hoy: Date): number {
  if (gastado <= 0) {
    return 0
  }
  const diaActual = hoy.getDate()
  const diasDelMes = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0).getDate()
  const promedioDiario = gastado / diaActual
  return Math.round(promedioDiario * diasDelMes * 100) / 100
}