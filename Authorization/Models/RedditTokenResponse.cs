namespace Authorization.Models;

public class RedditTokenResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public string? ExpiresIn { get; set; }
    public string? Scope { get; set; }
    public string? RefreshToken { get; set; }
}
