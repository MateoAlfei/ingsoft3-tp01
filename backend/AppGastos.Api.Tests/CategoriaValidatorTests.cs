using AppGastos.Api.Common;
using AppGastos.Api.Logica;
using Xunit;

namespace AppGastos.Api.Tests;

public class CategoriaValidatorTests
{
    // Regla 1: el nombre es obligatorio.
    // Tres formas de "no tener nombre": vacío, solo espacios y nulo (el campo no vino).
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NombreSinContenido_EsRechazado(string? nombre)
    {
        var ex = Assert.Throws<ValidationException>(
            () => CategoriaValidator.Validar(nombre, 1000m));

        Assert.Contains("requerido", ex.Message);
    }

    // Regla 2: el presupuesto no puede ser negativo.
    [Fact]
    public void PresupuestoNegativo_EsRechazado()
    {
        var ex = Assert.Throws<ValidationException>(
            () => CategoriaValidator.Validar("Comida", -1m));

        Assert.Contains("negativo", ex.Message);
    }

    // El otro lado del límite: 0 es válido ("no quiero gastar nada acá"),
    // y null también ("esta categoría no tiene presupuesto").
    [Theory]
    [InlineData(0)]
    [InlineData(null)]
    public void PresupuestoCeroOSinPresupuesto_EsAceptado(int? presupuesto)
    {
        var nombre = CategoriaValidator.Validar("Comida", presupuesto);

        Assert.Equal("Comida", nombre);
    }

    // La limpieza: los espacios de los costados no se guardan.
    [Fact]
    public void NombreConEspacios_SeGuardaRecortado()
    {
        var nombre = CategoriaValidator.Validar("  Transporte  ", null);

        Assert.Equal("Transporte", nombre);
    }
}