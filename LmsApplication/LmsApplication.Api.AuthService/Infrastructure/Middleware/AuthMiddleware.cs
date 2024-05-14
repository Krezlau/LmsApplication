namespace LmsApplication.Api.AuthService.Infrastructure.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger,
        IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }
}