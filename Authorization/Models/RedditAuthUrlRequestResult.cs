namespace Authorization.Models;

public class RedditAuthUrlRequestResult
{
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; } = "";
    public string AuthUrl { get; set; } = "";

    public RedditAuthUrlRequestResult() { }

    public RedditAuthUrlRequestResult(int statusCode, string statusMessage, string authUrl = "")
    {
        StatusCode = statusCode;
        StatusMessage = statusMessage;
        AuthUrl = authUrl;
    }
}
