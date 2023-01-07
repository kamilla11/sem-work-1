using System.Security.Cryptography;
using System.Text;

namespace HttpServer.Services;

public class Hash
{
// хеширование
    public static string ComputePasswordHash(string password)
    {
        byte[] saltBytes = UTF8Encoding.UTF8.GetBytes("Moroz i solnce; den' chudesnyi!");

        byte[] passwordBytes = UTF8Encoding.UTF8.GetBytes(password);

        byte[] preHashed = new byte[saltBytes.Length + passwordBytes.Length];
        Buffer.BlockCopy(passwordBytes, 0, preHashed, 0, passwordBytes.Length);
        Buffer.BlockCopy(saltBytes, 0, preHashed, passwordBytes.Length, saltBytes.Length);

        SHA1 sha1 = SHA1.Create();
        var hashPassword = sha1.ComputeHash(preHashed);
        return Convert.ToBase64String(hashPassword);
    }

// проверка хешированного пароля и введенного для авторизации
    public static bool IsPasswordValid(string passwordToValidate, string correctPasswordHash)
    {
        var hashedPassword = ComputePasswordHash(passwordToValidate);

        return hashedPassword.Equals(correctPasswordHash);
    }
}