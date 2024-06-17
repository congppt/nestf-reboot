using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace NestF.Infrastructure.Utils;

public static class StringUtil
{
    public static string Hash(this string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA512);
    }
    public static string HashWithNoSalt(this string origin)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(origin));
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
    public static bool VerifyHashString(this string hash, string password, HashType type = HashType.SHA512)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash, type);
    }
    public static string RemoveDiacritics(this string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);
        foreach (var c in normalizedString.Where(c =>
                     CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(c);
        }
        return stringBuilder.ToString().ToLower();
    }
    public static string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var password = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }
        return password.ToString();
    }

    public static string GenerateCacheKey<T>(int id) where T : class
    {
        return $"{nameof(T)}_{id}";
    }
}