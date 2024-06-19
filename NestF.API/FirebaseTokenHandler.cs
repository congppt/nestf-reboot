using System.Security.Claims;
using System.Text.Encodings.Web;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;

namespace Backend_API;

public class FirebaseTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private FirebaseAuth firebaseAuth;
    private readonly IAccountService accountService;
    private const string AUTHORIZATION_HEADER = "Authorization";
    private const string AUTHENTICATION_SCHEME = "Bearer ";

    public FirebaseTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        FirebaseApp firebase,
        IAccountService accountService) : base(options, logger, encoder)
    {
        this.firebaseAuth = FirebaseAuth.GetAuth(firebase);
        this.accountService = accountService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(AUTHORIZATION_HEADER,
                out Microsoft.Extensions.Primitives.StringValues value))
            return AuthenticateResult.NoResult();
        var bearerToken = value.ToString();
        if (string.IsNullOrWhiteSpace(bearerToken) || !bearerToken.StartsWith(AUTHENTICATION_SCHEME))
            return AuthenticateResult.Fail("msg");
        string token = bearerToken.Substring(AUTHENTICATION_SCHEME.Length);
        Dictionary<string, object>? claims = null;
        try
        {
            FirebaseToken firebaseToken = await firebaseAuth.VerifyIdTokenAsync(token);
            if (!firebaseToken.Claims.TryGetValue(ClaimConstants.PHONE, out var phone))
                return AuthenticateResult.Fail("Invalid token");
            if (!firebaseToken.Claims.TryGetValue(ClaimTypes.Role, out var temp))
            {
                var account = await accountService.GetCustomerByPhoneAsync((string) phone);
                if (account is { IsActive: false }) return AuthenticateResult.Fail("Blocked");
                if (account != null)
                {
                    claims ??= new Dictionary<string, object>
                    {
                        { ClaimTypes.Role, Role.Customer.ToString() },
                        { ClaimConstants.ID, account.Id },
                    };
                    await firebaseAuth.SetCustomUserClaimsAsync(firebaseToken.Uid, claims);
                }
            }

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(
                    new List<ClaimsIdentity>()
                    {
                        new ClaimsIdentity(ToClaims(firebaseToken.Claims),
                            ClaimConstants.PHONE)
                    }
                ),
                Scheme.Name
            );
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            var jwtScheme = await Context.AuthenticateAsync(DefaultConstants.STAFF_SCHEME);
            return jwtScheme;
        }
    }

    private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
    {
        var usableClaim = new List<Claim>
        {
            new Claim(ClaimConstants.PHONE, claims[ClaimConstants.PHONE].ToString()!)
        };
        var customClaims = new[]
            { ClaimTypes.Role, ClaimConstants.ID };
        foreach (var claim in customClaims)
        {
            if (claims.TryGetValue(claim, out var value)) usableClaim.Add(new Claim(claim, value.ToString()!));
        }
        return usableClaim;
    }
}