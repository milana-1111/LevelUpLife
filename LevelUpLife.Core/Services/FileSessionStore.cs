namespace LevelUpLife.Core.Services;

/// <summary>
/// Хранит токен сессии в текстовом файле.
/// </summary>
public class FileSessionStore : ISessionStore
{
    private readonly string _filePath;

    /// <summary>
    /// Создаёт хранилище, которое работает с указанным файлом.
    /// </summary>
    /// <param name="filePath">Полный путь к файлу с токеном.</param>
    public FileSessionStore(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Читает токен из файла.
    /// </summary>
    /// <returns>Токен или <c>null</c>, если файла нет или он не читается.</returns>
    public string? Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            string token = File.ReadAllText(_filePath).Trim();
            return token.Length == 0 ? null : token;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>
    /// Записывает токен в файл. Если папки нет, она создаётся.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    public void Save(string token)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_filePath, token);
    }

    /// <summary>
    /// Удаляет файл с токеном.
    /// </summary>
    public void Clear()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
        catch (IOException)
        {
            // Файл занят или недоступен: токен всё равно будет отвергнут сервисом после выхода.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
