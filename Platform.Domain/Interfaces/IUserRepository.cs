using Platform.Domain.Entities;

namespace Platform.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}
