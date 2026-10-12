using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// מחלקת עזר לניהול סיסמאות באמצעות PBKDF2
/// עם salt כדי למנוע התקפות rainbow table
/// </summary>
public static class PasswordHelper
{
    private const int SaltSize = 16; // 16 bytes for salt
    private const int HashSize = 32; // 32 bytes for hash
    private const int Iterations = 100_000; // PBKDF2 iterations

    /// <summary>
    /// יוצר hash בטוח לסיסמה עם salt
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty");

        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                byte[] hashWithSalt = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashWithSalt, 0, SaltSize);
                Array.Copy(hash, 0, hashWithSalt, SaltSize, HashSize);

                return Convert.ToBase64String(hashWithSalt);
            }
        }
    }

    /// <summary>
    /// מוודא שהסיסמה תואמת את ה-hash
    /// </summary>
    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            byte[] hashWithSalt = Convert.FromBase64String(hash);

            if (hashWithSalt.Length != SaltSize + HashSize)
                return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashWithSalt, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] computedHash = pbkdf2.GetBytes(HashSize);
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashWithSalt[i + SaltSize] != computedHash[i])
                        return false;
                }
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
