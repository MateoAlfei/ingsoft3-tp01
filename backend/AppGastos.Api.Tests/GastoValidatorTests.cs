using AppGastos.Api.Common;
using AppGastos.Api.Logica;
using Xunit;

namespace AppGastos.Api.Tests;

public class GastoValidatorTests
{
    // "Hoy" fijo: el test da lo mismo hoy y dentro de un año (determinismo).
    private static readonly DateOnly Hoy = new(2026, 10, 1);

    [Theory]
    [InlineData(0)]        // el borde exacto: 0 NO es "mayor a 0"
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void MontoNoPositivo_EsRechazado(double monto)
    {
        // Act + Assert: esperamos que tire la excepción
        var ex = Assert.Throws<ValidationException>(
            () => GastoValidator.Validar((decimal)monto, Hoy, Hoy));

        Assert.Contains("mayor a 0", ex.Message);
    }

    [Fact]
    public void FechaFutura_EsRechazada()
    {
        // Arrange
        var manana = Hoy.AddDays(1);

        // Act
        var ex = Assert.Throws<ValidationException>(
            () => GastoValidator.Validar(100m, manana, Hoy));

        // Assert
        Assert.Contains("futura", ex.Message);
    }

    [Fact]
    public void GastoConFechaDeHoy_EsAceptado()
    {
        // Act: Record.Exception devuelve la excepción si hubo una, o null si no
        var ex = Record.Exception(() => GastoValidator.Validar(100m, Hoy, Hoy));

        // Assert: no tiene que haber tirado nada
        Assert.Null(ex);
    }

    [Fact]
    public void MontoMinimoPositivo_EsAceptado()
    {
       var ex = Record.Exception(() => GastoValidator.Validar(0.01m, Hoy.AddDays(-1), Hoy));
//                                                             
        Assert.Null(ex);
    }
}