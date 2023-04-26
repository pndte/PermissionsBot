namespace PermissionsBot.Tokens;

using System.Security.Cryptography;
using System.Text;

public static class TokenManager
{
    private static readonly Lazy<Random> _random = new Lazy<Random>();
    private static readonly SHA256 _sha256 = SHA256.Create();

    public static string CreateTeacherAccessToken()
    {
        string textToEncode =
            $"{_random.Value.Next(Int32.MinValue, Int32.MaxValue) + _random.Value.Next(Int32.MinValue, Int32.MaxValue)} teacher";

        textToEncode = GetHash(textToEncode);
        return $"[СОШ6]:{GetHash(textToEncode)}_TEACHER";
    }

    public static string CreateAdminAccessToken()
    {
        string textToEncode =
            $"{_random.Value.Next(Int32.MinValue, Int32.MaxValue) - _random.Value.Next(Int32.MinValue, Int32.MaxValue) + _random.Value.Next(Int32.MinValue, Int32.MaxValue)} admin";

        textToEncode = GetHash(textToEncode);
        return $"[СОШ6]:{GetHash(textToEncode)}_ADMIN";
    }

    private static string GetHash(string textToEncode)
    {
        var returnedHash = new StringBuilder();
        byte[] computedHash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(textToEncode));
        foreach (byte theByte in computedHash)
        {
            returnedHash.Append(theByte.ToString("x2"));
        }

        return returnedHash.ToString();
    }
}