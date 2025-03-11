namespace AuthenticationService.Services
{
    public interface ITokenGenerator
    {
        string generateToken(string email, string role);
    }
}