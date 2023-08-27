using System.Security.Cryptography;
using System.Web;

namespace Authorization.Models;

public class RedditClientSettings
{
    public string AuthorizationUri { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string AccessUri { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;

    public RedditClientSettings() { }

    public RedditClientSettings(string authorizationUri, string clientId, string secret, string redirectUri, string accessUri, string scopes)
    {
        AuthorizationUri = authorizationUri;
        ClientId = clientId;
        Secret = secret;
        RedirectUri = redirectUri;
        AccessUri = accessUri;
        Scopes = scopes;
    }

    public string ToAuthorizationString()
    {
        string authState = CreateSecureRandomString();
        string encodedRedirectUrl = HttpUtility.UrlEncode(RedirectUri);

        return $"{AuthorizationUri}?client_id={ClientId}&response_type=code&state={authState}&redirect_uri={encodedRedirectUrl}&duration=permanent&scope={Scopes}";
    }

    /// <summary>
    /// Creates a cryptographically secure random key string.
    /// </summary>
    /// <param name="count">The number of bytes of random values to create the string from</param>
    /// <returns>A secure random string</returns>
    private static string CreateSecureRandomString(int count = 64) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
}
