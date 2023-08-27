namespace Authorization.Models;

public class RedditAccessInfoRequestResult
{
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; } = "";
    public string RedirectUrl { get; set; } = "";
    public string AccessUrl { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string Secret { get; set; } = "";

    public RedditAccessInfoRequestResult() { }

    public RedditAccessInfoRequestResult(int statusCode, string statusMessage, string redirectUrl = "", string accessUrl = "", string clientId = "", string secret = "")
    {
        StatusCode = statusCode;
        StatusMessage = statusMessage;
        RedirectUrl = redirectUrl;
        AccessUrl = accessUrl;
        ClientId = clientId;
        Secret = secret;
    }
}
