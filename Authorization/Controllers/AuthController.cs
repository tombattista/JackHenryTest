using Authorization.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public RedditAuthUrlRequestResult IndexAsync()
    {
        RedditClientSettings? clientSettings = _configuration.GetSection("RedditClient").Get<RedditClientSettings>();

        return new RedditAuthUrlRequestResult()
        {
            StatusCode = clientSettings is null ? 500 : 200,
            StatusMessage = clientSettings is null ? "Settings not configured." : "AuthUrl successfully retrieved. Ready to verify authorization.",
            AuthUrl = clientSettings is null ? string.Empty : clientSettings.ToAuthorizationString()
        };
    }

    [HttpGet("AccessInfo/")]
    public RedditAccessInfoRequestResult GetAccessInfo()
    {
        RedditClientSettings? clientSettings = _configuration.GetSection("RedditClient").Get<RedditClientSettings>();

        return new RedditAccessInfoRequestResult()
        {
            StatusCode = clientSettings is null ? 500 : 200,
            StatusMessage = clientSettings is null ? "Settings not configured." : "AccessInfo successfully retrieved. Ready to retrieve token.",
            RedirectUrl = clientSettings is null ? string.Empty : clientSettings.RedirectUri,
            AccessUrl = clientSettings is null ? string.Empty : clientSettings.AccessUri,
            ClientId = clientSettings is null ? string.Empty : clientSettings.ClientId,
            Secret = clientSettings is null ? string.Empty : clientSettings.Secret
        };
    }
}
