using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LmsApplication.Api.AuthService.Infrastructure.Auth;

public class TenantJwtBearerHandler : JwtBearerHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public TenantJwtBearerHandler(IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TenantJwtBearerHandler(IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        SetupTokenValidationParameters();
        return base.HandleAuthenticateAsync();
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        return base.HandleChallengeAsync(properties);
    }

    protected override string? ResolveTarget(string? scheme)
    {
        return base.ResolveTarget(scheme);
    }

    protected override Task<object> CreateEventsAsync()
    {
        return base.CreateEventsAsync();
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        return base.HandleForbiddenAsync(properties);
    }

    protected override Task InitializeEventsAsync()
    {
        return base.InitializeEventsAsync();
    }

    protected override Task InitializeHandlerAsync()
    {
        return base.InitializeHandlerAsync();
    }

    private void SetupTokenValidationParameters()
    {
        var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["TenantId"];
        var tenantId = tenantIdHeader.ToString();
        if (string.IsNullOrEmpty(tenantId))
        {
            return;
        }

        Options.Audience = tenantId;
        Options.TokenValidationParameters.ValidAudience = tenantId;
    }
}