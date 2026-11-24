using System.Security.Cryptography;

namespace LevelUpLife.Core.Security;

/// <summary>
/// Хеширование паролей алгоритмом PBKDF2 (SHA-256) со случайной солью.
/// Формат хеша: «число итераций.соль.ключ» (соль и ключ в Base64).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    /// <summary>
    /// Создаёт хеш пароля со случайной солью.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Строка с числом итераций, солью и хешем.</returns>
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    /// <summary>
    /// Проверяет, соответствует ли пароль сохранённому хешу.
    /// </summary>
    /// <param name="password">Введённый пароль.</param>
    /// <param name="hash">Сохранённый хеш в формате «итерации.соль.ключ».</param>
    /// <returns><c>true</c>, если пароль верный; иначе <c>false</c>.</returns>
    public bool Verify(string password, string hash)
    {
        string[] parts = hash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations) || iterations <= 0)
        {
            return false;
        }

        try
        {
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expected = Convert.FromBase64String(parts[2]);
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
