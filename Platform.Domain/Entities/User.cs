using Platform.Domain.Exceptions;

namespace Platform.Domain.Entities;

/// <summary>
/// Represents a user capable of logging into the system.
/// </summary>
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // Required by EF Core
    private User() { }

    public static User Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("El password hash es obligatorio.");

        // Básica validación de formato
        if (!email.Contains('@'))
            throw new DomainException("El email no tiene un formato válido.");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash
        };
    }
}
