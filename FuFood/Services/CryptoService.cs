using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

public class CryptoService
{
    public CryptoService(IOptions<CryptoOptions> options)
    {
        _options = options.Value;
        AccessTokenSigner = Derive("access-token-signer");
        InvitationTokenHmacKey = Derive("invitation-token-hmac-key");
    }

    private readonly CryptoOptions _options;
    private const string Salt = "富食品版權所有,不要再猜我們的秘密值";
    private readonly byte[] _saltBytes = Encoding.UTF8.GetBytes(Salt);

    public byte[] AccessTokenSigner { get; }
    private byte[] InvitationTokenHmacKey { get; }

    private byte[] Derive(string info, int length = 32)
    {
        return HKDF.DeriveKey(HashAlgorithmName.SHA256, outputLength: length, salt: _saltBytes,
            ikm: _options.SecretKeyBase, info: Encoding.UTF8.GetBytes(info));
    }

    public byte[] HashInvitationToken(string token)
    {
        using var hmac = new HMACSHA256(InvitationTokenHmacKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
    }
}