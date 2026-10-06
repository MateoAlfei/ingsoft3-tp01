using AppGastos.Api.Common;

namespace AppGastos.Api.Logica;

public static class GastoValidator
{
    public static void Validar(decimal monto, DateOnly fecha, DateOnly hoy)
    {
        if (monto <= 0)
            throw new ValidationException("El monto debe ser mayor a 0.");

        if (fecha > hoy)
            throw new ValidationException("La fecha no puede ser futura.");
    }
}