using AppGastos.Api.Common;

namespace AppGastos.Api.Logica;

public static class RegistroValidator
{
    // El largo mínimo vive en UN solo lugar. El mensaje y los tests lo leen de acá.
    public const int LargoMinimoPassword = 6;

    // Valida los datos de registro y devuelve el email ya normalizado.
    public static string Validar(string? email, string? password, string? nombre)
    {
        // El "?." evita que explote si el email viene null: en ese caso todo da null.
        var emailNormalizado = email?.Trim().ToLowerInvariant();

        // Regla 1: el email tiene que existir y tener un @
        if (string.IsNullOrWhiteSpace(emailNormalizado) || !emailNormalizado.Contains('@'))
            throw new ValidationException("El email no es válido.");

        // Regla 2: la contraseña tiene que tener al menos 6 caracteres
        if (string.IsNullOrWhiteSpace(password) || password.Length < LargoMinimoPassword)
            throw new ValidationException($"La contraseña debe tener al menos {LargoMinimoPassword} caracteres.");

        // Regla 3: el nombre es obligatorio
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("El nombre es requerido.");

        return emailNormalizado;
    }
}