using AppGastos.Api.Logica;
using Xunit;

namespace AppGastos.Api.Tests;

public class PresupuestoTests
{
    // Camino 1: sin presupuesto no hay restante.
    [Fact]
    public void CategoriaSinPresupuesto_NoTieneRestante()
    {
        var restante = Presupuesto.Restante(null, 500m);

        Assert.Null(restante);
    }

    // Camino 2: con presupuesto, restante = presupuesto − gastado.
    [Theory]
    [InlineData(1000, 300, 700)]     // quedó plata
    [InlineData(1000, 1000, 0)]      // borde: gastó justo el presupuesto
    [InlineData(1000, 1500, -500)]   // se pasó: el restante es NEGATIVO, no se redondea a 0
    public void Restante_EsPresupuestoMenosGastado(int presupuesto, int gastado, int esperado)
    {
        var restante = Presupuesto.Restante(presupuesto, gastado);

        Assert.Equal((decimal?)esperado, restante);
    }
}