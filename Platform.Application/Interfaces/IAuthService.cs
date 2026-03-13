using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}
