namespace Rystem.Security.Cryptography
{
    public sealed record AesCryptoOptions(string Password, string SaltKey, string VIKey);
}