namespace LevelUpLife.Core.Security;

/// <summary>
/// Хеширование и проверка паролей. Пароли в базе не хранятся.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Создаёт хеш пароля со случайной солью.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Строка с параметрами, солью и хешем.</returns>
    string Hash(string password);

    /// <summary>
    /// Проверяет, соответствует ли пароль сохранённому хешу.
    /// </summary>
    /// <param name="password">Введённый пароль.</param>
    /// <param name="hash">Сохранённый хеш.</param>
    /// <returns><c>true</c>, если пароль верный; иначе <c>false</c>.</returns>
    bool Verify(string password, string hash);
}
