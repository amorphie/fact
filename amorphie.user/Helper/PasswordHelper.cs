using Konscious.Security.Cryptography;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class PasswordHelper
{
  
//   public byte[] CreateHash(byte[] password, byte[] salt)
//     {
//         using var argon2 = new Argon2id(password);
//         argon2.Salt = salt;
//         argon2.DegreeOfParallelism = 8;
//         argon2.Iterations = 4;
//         argon2.MemorySize = 1024 * 128;

//         return argon2.GetBytes(32);                
//     }
        
    // public bool VerifyHash(byte[] password, byte[] salt, byte[] hash) => 
    //     HashPassword(password, salt).SequenceEqual(hash);

    // public static byte[] GenerateSalt()
    // {
    //     var buffer = new byte[32];
    //     using var rng = new RNGCryptoServiceProvider();
    //     rng.GetBytes(buffer);
    //     return buffer;
    // }
    //  private byte[] HashPassword(string input, byte[] salt)
    // {
    //     Argon2id argon2 = new Argon2id(Encoding.UTF8.GetBytes(input));

    //     argon2.Salt = salt;
    //     argon2.DegreeOfParallelism = 8; // four cores
    //     argon2.Iterations = 4;
    //     argon2.MemorySize = 1024 * 1024; // 1 GB

    //     return argon2.GetBytes(16);
    // }
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
            argon2id.MemorySize =  1024 * 1024;
            return argon2id.GetBytes(16);
        }

        public static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }

