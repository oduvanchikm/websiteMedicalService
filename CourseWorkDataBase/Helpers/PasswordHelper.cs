using System.Text;
using System.Security.Cryptography;

namespace CourseWorkDataBase.Helpers;

public class PasswordHelper
{
    public static string HashPassword(string password, string login)
    {
        byte[] salt;
        using (var range = new RNGCryptoServiceProvider())
        {
            salt = new byte[16];
            range.GetBytes(salt);
        }
        
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password + login);
        byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
        
        Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, saltedPassword.Length);
        Buffer.BlockCopy(salt, 0, saltedPassword, saltedPassword.Length, saltedPassword.Length);

        using (SHA256 sha256Hash = SHA256.Create())
        {
            return Convert.ToBase64String(sha256Hash.ComputeHash(saltedPassword));
        }
    }
}