using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

public class CryptoService(IOptions<CryptoOptions> options)
{
    private readonly CryptoOptions _options = options.Value;
    private const string Salt = "富食品版權所有,不要再猜我們的秘密值";
    private readonly byte[] _saltBytes = Encoding.UTF8.GetBytes(Salt);

    public byte[] AccessTokenSigner =>
        HKDF.DeriveKey(HashAlgorithmName.SHA256, outputLength: 32, salt: _saltBytes, ikm: _options.SecretKeyBase,
            info: "access-token-signer"u8.ToArray());
}