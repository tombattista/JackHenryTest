using Authorization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Text.Unicode;
using System.Net.Http.Headers;

namespace JHPopularStats.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly string _authStateCookieName = "AuthState";
    private readonly CookieOptions _cookieOptions = new()
    {
        Path = "/",
        Secure = true,
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddHours(1)
    };

    [BindProperty]
    public string Message { get; private set; } = "Authorized";

    [BindProperty]
    public List<string> PopularPosts { get; private set; } = new();

    [BindProperty]
    public List<string> HighPostingUsers { get; private set; } = new();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        Message = "Waiting for authorization...";
    }

    public async Task<IActionResult> OnGet(string? state = null)
    {
        string? authState = Request.Cookies[_authStateCookieName];
        bool authCookieExists = authState is not null;

        if (!authCookieExists && state is null)
        {
            RedditAuthUrlRequestResult authUrlResult = await GetAuthTokenUrl() ?? new RedditAuthUrlRequestResult(500, "No authorization token request returned.");

            Message = authUrlResult.StatusMessage;

            if (authUrlResult.StatusCode != 200)
            {
                _logger.LogError(authUrlResult.StatusMessage);
                return Page();
            }

            _logger.LogInformation(authUrlResult.StatusMessage);
            return Redirect(authUrlResult.AuthUrl);
        }
        
        if (!authCookieExists && state is not null)
        {
            WriteCookie(_authStateCookieName, state);
            authState = state;
        }

        if (authState is not null)
        {
            // Get access token
            RedditTokenResponse? accessTokenResponse = await GetAccessToken(authState);

            if (accessTokenResponse is not null && accessTokenResponse.AccessToken is not null)
            {
                // Get voting stats, using authState
                PostVotingStats(accessTokenResponse.AccessToken);

                // Get user stats, using authState
                PostUserStats(accessTokenResponse.AccessToken);
            }
        }

        return Page();
    }

    private void WriteCookie(string key, string value)
    {
        Response.Cookies.Append(key, value, _cookieOptions);
    }

    private static async Task<RedditAuthUrlRequestResult?> GetAuthTokenUrl()
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri("https://localhost:7001/")
        };

        HttpResponseMessage response = await client.GetAsync("auth/");
        return await response.Content.ReadFromJsonAsync<RedditAuthUrlRequestResult>();
    }

    private static async Task<RedditAccessInfoRequestResult?> GetAccessInfo()
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:7001/")
        };

        HttpResponseMessage response = await httpClient.GetAsync("auth/AccessInfo");
        return await response.Content.ReadFromJsonAsync<RedditAccessInfoRequestResult>();
    }

    private static async Task<RedditTokenResponse?> GetAccessToken(string authState)
    {
        RedditAccessInfoRequestResult? accessInfoResult = await GetAccessInfo();
        if (accessInfoResult is null)
        {
            return null;
        }

        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(accessInfoResult.AccessUrl)
        };
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.ConnectionClose = true;

        List<KeyValuePair<string, string>> values = new()
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", authState),
            new KeyValuePair<string, string>("redirect_uri", accessInfoResult.RedirectUrl)
        };

        var content = new FormUrlEncodedContent(values);

        var authenticationString = $"{accessInfoResult.ClientId}:{accessInfoResult.Secret}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString));

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/");
        requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("JHPopularStats", "1.0"));
        requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("(+https://localhost:7001/)"));
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = content;

        using HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<RedditTokenResponse>();
    }

    private void PostVotingStats(string accessToken)
    {
        PopularPosts.Add(accessToken);
    }

    private void PostUserStats(string accessToken)
    {

    }
}