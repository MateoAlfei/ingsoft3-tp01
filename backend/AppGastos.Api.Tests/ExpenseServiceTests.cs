using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Models;
using AppGastos.Api.Services;
using Moq;
using Xunit;

namespace AppGastos.Api.Tests;

public class ExpenseServiceTests
{
    // Datos de prueba que comparten los tests
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Category Comida = new() { Id = Guid.NewGuid(), UserId = UserId, Name = "Comida" };
    private static readonly DateOnly UnaFechaPasada = new(2026, 1, 15);

    // Caso feliz: un gasto válido se guarda UNA sola vez
    [Fact]
    public async Task CrearGastoValido_LoGuardaUnaSolaVez()
    {
        // Arrange
        // 1. Fabricamos el impostor del repositorio
        var repo = new Mock<IGastoRepository>();

        // 2. Le enseñamos qué contestar: "si te piden la categoría Comida de este usuario, devolvela"
        //    (en este papel el doble es un STUB: solo provee datos)
        repo.Setup(r => r.BuscarCategoriaAsync(UserId, Comida.Id)).ReturnsAsync(Comida);

        // 3. Le damos el impostor al servicio, en lugar del repositorio real
        var servicio = new ExpenseService(repo.Object);

        // Act
        await servicio.CreateAsync(UserId, new CreateExpenseRequest(Comida.Id, 1500m, "super", UnaFechaPasada));

        // Assert: NO miramos lo que devolvió el método. Le preguntamos al impostor CÓMO lo usaron:
        // "¿te pidieron agregar un gasto de $1500, de este usuario y de esta categoría, exactamente una vez?"
        // (en este papel el doble es un MOCK: verificamos la interacción)
        repo.Verify(r => r.AgregarAsync(It.Is<Expense>(e =>
                e.Amount == 1500m && e.UserId == UserId && e.CategoryId == Comida.Id)),
            Times.Once);
    }

    // Caso de error: con un monto inválido el servicio corta ANTES de tocar la base
    [Fact]
    public async Task CrearGastoConMontoInvalido_NoTocaLaBase()
    {
        // Arrange: un impostor al que no le enseñamos nada
        var repo = new Mock<IGastoRepository>();
        var servicio = new ExpenseService(repo.Object);

        // Act + Assert: un monto de 0 tiene que tirar ValidationException
        await Assert.ThrowsAsync<ValidationException>(() =>
            servicio.CreateAsync(UserId, new CreateExpenseRequest(Comida.Id, 0m, null, UnaFechaPasada)));

        // Assert: y lo más importante, NUNCA se intentó guardar nada
        repo.Verify(r => r.AgregarAsync(It.IsAny<Expense>()), Times.Never);
    }
}
