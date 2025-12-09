using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

// 處理每個請求(cookie 和資料庫)的驗證
public class AccessTokenHandler(
    IOptionsMonitor<AccessTokenHandler.AccessTokenHandlerOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder)
    : AuthenticationHandler<AccessTokenHandler.AccessTokenHandlerOptions>(options, loggerFactory, encoder)
{
    public class AccessTokenHandlerOptions : AuthenticationSchemeOptions
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return AuthenticateResult.Fail("用戶驗證錯誤");
    }
}