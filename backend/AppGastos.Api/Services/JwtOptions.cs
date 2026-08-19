namespace AppGastos.Api.Services;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "AppGastos";
    public int ExpirationMinutes { get; set; } = 60 * 12;
}
