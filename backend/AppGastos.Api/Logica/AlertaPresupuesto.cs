namespace AppGastos.Api.Logica;

public static class AlertaPresupuesto
{
    public const decimal UmbralAlerta = 0.8m;

    public static string Nivel(decimal? presupuestoMensual, decimal gastado)
    {
        if (presupuestoMensual is null || presupuestoMensual <= 0)
        {
            return "sin-presupuesto";
        }

        var usado = gastado / presupuestoMensual.Value;

        if (usado >= 1m)
        {
            return "excedido";
        }

        if (usado >= UmbralAlerta)
        {
            return "alerta";
        }

        return "ok";
    }

    public static string Mensaje(string categoria, string nivel)
    {
        return nivel switch
        {
            "excedido" => $"Te pasaste del presupuesto de {categoria}.",
            "alerta" => $"Estás cerca del límite de {categoria}.",
            "sin-presupuesto" => $"{categoria} no tiene presupuesto definido.",
            _ => $"{categoria} está dentro del presupuesto."
        };
    }
}