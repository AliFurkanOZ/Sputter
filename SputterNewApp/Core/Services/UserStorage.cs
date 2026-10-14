using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SputterNewApp.Core.Services
{
    public static class UserStorage
    {
        private static readonly SemaphoreSlim _fileLock = new(1, 1);

        public static string AppRoot =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SputterNewApp",
                "Users"
            );

        public static string GetUserDir(string userId)
            => Path.Combine(AppRoot, userId);

        public static string GetUserFile(string userId, string fileName)
            => Path.Combine(GetUserDir(userId), fileName);

        public static async Task<T> LoadAsync<T>(string path, T fallback)
        {
            await _fileLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!File.Exists(path))
                    return fallback;

                var json = await File.ReadAllTextAsync(path).ConfigureAwait(false);
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

        public static T LoadSync<T>(string path, T fallback)
        {
            try
            {
                if (!File.Exists(path))
                    return fallback;

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json) ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }

        public static async Task SaveAsync<T>(string path, T data)
        {
            await _fileLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(
                    data,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserStorage SaveAsync error for {path}: {ex.Message}");
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}
