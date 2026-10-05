using AppGastos.Api.Common;
using AppGastos.Api.Logica;
using Xunit;

namespace AppGastos.Api.Tests;

public class RegistroValidatorTests
{
    // Regla 1: el email tiene que existir y tener @.
    [Theory]
    [InlineData("")]                    // vacío
    [InlineData("mateo.gmail.com")]   // sin @
    public void EmailInvalido_EsRechazado(string email)
    {
        var ex = Assert.Throws<ValidationException>(
            () => RegistroValidator.Validar(email, "secreta", ""));

        Assert.Contains("email", ex.Message);
    }

    // Regla 2: contraseña demasiado corta. El mensaje tiene que decir cuál es el mínimo.
    [Fact]
    public void PasswordMasCortaQueElMinimo_ExplicaElMinimo()
    {
        // Arrange: una contraseña con UN carácter menos que el mínimo (5)
        var corta = new string('x', RegistroValidator.LargoMinimoPassword - 1);

        // Act
        var ex = Assert.Throws<ValidationException>(
            () => RegistroValidator.Validar("a@b.com", corta, "mateo"));

        // Assert: el mensaje menciona el número mínimo ("6")
        Assert.Contains(RegistroValidator.LargoMinimoPassword.ToString(), ex.Message);
    }

    // El otro lado del límite: exactamente el mínimo (6) tiene que aceptarse.
    [Fact]
    public void PasswordDelLargoMinimoExacto_EsAceptada()
    {
        var justa = new string('x', RegistroValidator.LargoMinimoPassword);

        var ex = Record.Exception(
            () => RegistroValidator.Validar("a@b.com", justa, "mateo"));

        Assert.Null(ex);
    }

    // Regla 3: el nombre es obligatorio.
    [Fact]
    public void NombreVacio_EsRechazado()
    {
        var ex = Assert.Throws<ValidationException>(
            () => RegistroValidator.Validar("a@b.com", "secreta", " "));

        Assert.Contains("nombre", ex.Message);
    }

    // La normalización: el email se guarda en minúsculas y sin espacios.
    [Fact]
    public void Email_SeNormalizaAMinusculasSinEspacios()
    {
        var email = RegistroValidator.Validar("  mateo@Mail.COM ", "secreta", "Facu");

        Assert.Equal("mateo@mail.com", email);
    }
}