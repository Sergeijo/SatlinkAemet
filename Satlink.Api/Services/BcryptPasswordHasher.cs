using BCrypt.Net;

using Satlink.Logic;

namespace Satlink.Api.Services;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
