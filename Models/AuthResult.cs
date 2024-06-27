namespace TodoAPI.Models;

public class AuthResult(string idToken, double expiresIn)
{
    public string IdToken { get; } = idToken;
    public double ExpiresIn { get; } = expiresIn;
}