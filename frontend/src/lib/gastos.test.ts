import { describe, expect, it, vi } from "vitest";
import type { DashboardSummary } from "../api/types";
import {
  esGastoValido,
  porcentajeUsado,
  resumenDelMes,
  totalGastado,
  type FormGasto,
  type ObtenerResumen,
} from "./gastos";

// ── Regla 2: validación del formulario de gasto ────────────────────────────
describe("esGastoValido", () => {
  // "Hoy" fijo, igual que en el backend: el test no depende del reloj.
  const HOY = "2026-10-01";

  // Un formulario válido de base. Cada caso de abajo le rompe UNA sola cosa.
  const valido: FormGasto = { categoryId: "cat-1", amount: "1500", date: HOY };

  // PARAMETRIZADO + CASO DE ERROR: el equivalente de [Theory] + [InlineData]
  it.each<{ caso: string; form: FormGasto }>([
    { caso: "sin categoría", form: { ...valido, categoryId: "" } },
    { caso: "con monto 0", form: { ...valido, amount: "0" } },
    { caso: "con monto negativo", form: { ...valido, amount: "-10" } },
    { caso: "sin monto", form: { ...valido, amount: "" } },
    { caso: "sin fecha", form: { ...valido, date: "" } },
    { caso: "con fecha futura", form: { ...valido, date: "2026-10-02" } },
  ])("rechaza un gasto $caso", ({ form }) => {
    expect(esGastoValido(form, HOY)).toBe(false);
  });

  // BORDE: hoy y el monto mínimo tienen que aceptarse
  it("acepta un gasto de hoy con el monto mínimo", () => {
    expect(esGastoValido({ ...valido, amount: "0.01" }, HOY)).toBe(true);
  });
});

// ── Regla 1: total de los gastos ───────────────────────────────────────────
describe("totalGastado", () => {
  it("suma los montos de todos los gastos", () => {
    expect(totalGastado([{ amount: 100 }, { amount: 250.5 }])).toBe(350.5);
  });

  it("una lista vacía da 0", () => {
    expect(totalGastado([])).toBe(0);
  });
});

// ── Regla 3: porcentaje de la barra de presupuesto ─────────────────────────
describe("porcentajeUsado", () => {
  it("sin presupuesto no hay porcentaje", () => {
    expect(porcentajeUsado(500, null)).toBeNull();
  });

  it.each<{ gastado: number; esperado: number }>([
    { gastado: 250, esperado: 25 },    // un cuarto
    { gastado: 1000, esperado: 100 },  // borde: justo el presupuesto
    { gastado: 1500, esperado: 100 },  // se pasó: la barra NO pasa del 100
  ])("con presupuesto 1000 y gastado $gastado da $esperado%", ({ gastado, esperado }) => {
    expect(porcentajeUsado(gastado, 1000)).toBe(esperado);
  });
});

// ── Regla 4: pedir el resumen del mes actual (MOCK) ────────────────────────
describe("resumenDelMes", () => {
  const resumenFalso: DashboardSummary = { month: 10, year: 2026, totalSpent: 0, categories: [] };

  it("pide a la API el mes actual: octubre es 10, no 9", async () => {
    // Arrange: el impostor de la API. vi.fn() fabrica una función falsa,
    // y mockResolvedValue le dice qué contestar (como el Setup de Moq).
    const obtener = vi.fn<ObtenerResumen>().mockResolvedValue(resumenFalso);
    const hoy = new Date(2026, 9, 15); // 15 de octubre (en JavaScript el mes 9 ES octubre)

    // Act
    const resultado = await resumenDelMes(obtener, hoy);

    // Assert: miramos CÓMO se usó la dependencia (como el Verify de Moq)
    expect(obtener).toHaveBeenCalledTimes(1);
    expect(obtener).toHaveBeenCalledWith(10, 2026);
    expect(resultado).toBe(resumenFalso);
  });

  it("en enero pide el mes 1 (borde: el primer mes)", async () => {
    const obtener = vi.fn<ObtenerResumen>().mockResolvedValue(resumenFalso);

    await resumenDelMes(obtener, new Date(2026, 0, 10)); // 10 de enero

    expect(obtener).toHaveBeenCalledWith(1, 2026);
  });
});