namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string message)> RegisterAsync(string username, string email, string password, string confirmPassword);
        Task<(bool success, string message)> LoginAsync(string usernameOrEmail, string password);
        Task LogoutAsync();
    }
}
