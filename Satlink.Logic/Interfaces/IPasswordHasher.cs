namespace Satlink.Logic;

/// <summary>
/// Port for password hashing/verification.
/// </summary>
public interface IPasswordHasher
{
    bool Verify(string password, string passwordHash);
}
