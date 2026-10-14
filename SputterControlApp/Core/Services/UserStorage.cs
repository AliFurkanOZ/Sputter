using System.Text.Json;
using System.Threading;
public static class UserStorage
{
    private static readonly System.Threading.SemaphoreSlim _fileLock = new(1, 1);
    public static string AppRoot =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SputterControlApp",
            "Users"
        );

    public static string GetUserDir(string userId)
        => Path.Combine(AppRoot, userId);

    public static string GetUserFile(string userId, string fileName)
        => Path.Combine(GetUserDir(userId), fileName);

    public static async Task<T> LoadAsync<T>(string path, T fallback)
    {
        await _fileLock.WaitAsync();
        try
        {
            if (!File.Exists(path))
                return fallback;

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<T>(json) ?? fallback;
        }
        catch (IOException)
        {
            return fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public static async Task SaveAsync<T>(string path, T data)
    {
        await _fileLock.WaitAsync();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var json = System.Text.Json.JsonSerializer.Serialize(
                data,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            );

            // Atomic write: önce temp'e yaz, sonra replace et
            var tmp = path + ".tmp";
            await File.WriteAllTextAsync(tmp, json);

            File.Move(tmp, path, overwrite: true);
        }
        finally
        {
            _fileLock.Release();
        }
    }

}
