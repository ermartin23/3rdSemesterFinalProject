using Microsoft.AspNetCore.Identity;

namespace api.Features.Auth;


public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string hashedPassword, string providedPassword);
}

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
        => _hasher.HashPassword(new object(), password);

    public bool Verify(string hashedPassword, string providedPassword)
        => _hasher.VerifyHashedPassword(new object(), hashedPassword, providedPassword)
            is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;

}