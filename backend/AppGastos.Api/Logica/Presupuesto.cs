namespace AppGastos.Api.Logica;

public static class Presupuesto
{
    // Cuánto queda del presupuesto mensual de una categoría.
    // - Sin presupuesto (null) → no hay "restante" que mostrar → null.
    // - Con presupuesto → presupuesto − gastado. Puede dar NEGATIVO: significa que te pasaste.
    public static decimal? Restante(decimal? presupuestoMensual, decimal gastado)
    {
        if (presupuestoMensual is null)
            return null;

        return presupuestoMensual - gastado;
    }
}