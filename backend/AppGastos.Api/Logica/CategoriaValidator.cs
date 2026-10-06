using AppGastos.Api.Common;

namespace AppGastos.Api.Logica;

public static class CategoriaValidator
{
    // Valida los datos de una categoría y devuelve el nombre ya limpio.
    public static string Validar(string? nombre, decimal? presupuestoMensual)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("El nombre de la categoría es requerido.");

        if (presupuestoMensual is < 0)
            throw new ValidationException("El presupuesto mensual no puede ser negativo.");

        return nombre.Trim();
    }
}