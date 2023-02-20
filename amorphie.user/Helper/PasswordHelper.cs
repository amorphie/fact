using Konscious.Security.Cryptography;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class PasswordHelper
{

    public static byte[] CreateSalt()
    {
        var buffer = new byte[16];
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(buffer);

        return buffer;
    }
    public static byte[] HashPassword(string password, byte[] salt)
    {
        var argon2id = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2id.Salt = salt;
        argon2id.DegreeOfParallelism = 8;
        argon2id.Iterations = 4;
        argon2id.MemorySize = 1024 * 1024;
        return argon2id.GetBytes(16);
    }

    public static bool VerifyHash(string password, byte[] salt, byte[] hash)
    {
        var newHash = HashPassword(password, salt);
        return hash.SequenceEqual(newHash);
    }
}

