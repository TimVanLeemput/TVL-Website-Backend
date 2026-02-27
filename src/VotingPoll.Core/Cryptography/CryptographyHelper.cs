using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace VotingPoll.Core.Cryptography;

public static class CryptographyHelper
{
    public static byte[] GenerateSalt(int length)
    {
        byte[] salt = new byte[length];
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
    
    public static byte[] HashPassword(string password, byte[] salt)
    {
        using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = 8; // Number of threads
            hasher.MemorySize = 65536; // 64 MB of memory
            hasher.Iterations = 4; // Number of iterations
            return hasher.GetBytes(128); // Get 128-byte hash
        }
    }
}