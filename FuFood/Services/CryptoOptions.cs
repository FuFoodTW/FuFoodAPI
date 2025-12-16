namespace FuFood.Services;

public class CryptoOptions
{
    public const string SectionName = "Crypto";
    public required byte[] SecretKeyBase { get; set; }
}