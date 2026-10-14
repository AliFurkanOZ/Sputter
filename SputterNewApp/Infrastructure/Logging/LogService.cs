using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SputterNewApp.Core.Models;

namespace SputterNewApp.Infrastructure.Logging
{
    public sealed class LogService
    {
        public int Capacity { get; }
        public string StorageFilePath { get; }

        private readonly LinkedList<LogEvent> _items = new();
        private readonly object _lock = new();

        private readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        public event Action<LogEvent>? OnNewEvent;

        public LogService(int capacity = 2000, string? storageFilePath = null)
        {
            Capacity = Math.Max(100, capacity);
            StorageFilePath = ResolveStoragePath(storageFilePath);
            EnsureStorageDirectory();
            LoadPersistedEvents();
        }

        public void Publish(LogEvent ev)
        {
            lock (_lock)
            {
                _items.AddLast(ev);
                while (_items.Count > Capacity)
                    _items.RemoveFirst();

                PersistAllUnsafe();
            }

            OnNewEvent?.Invoke(ev);
        }

        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
                PersistAllUnsafe();
            }
        }

        public List<LogEvent> SnapshotNewestFirst()
        {
            lock (_lock)
                return _items.Reverse().ToList();
        }

        private void EnsureStorageDirectory()
        {
            var dir = Path.GetDirectoryName(StorageFilePath);
            if (string.IsNullOrWhiteSpace(dir))
                return;

            Directory.CreateDirectory(dir);
        }

        private void LoadPersistedEvents()
        {
            if (!File.Exists(StorageFilePath))
                return;

            try
            {
                var json = File.ReadAllText(StorageFilePath);
                var items = JsonSerializer.Deserialize<List<LogEvent>>(json, _jsonOpts);
                if (items == null)
                    return;

                lock (_lock)
                {
                    foreach (var item in items.TakeLast(Capacity))
                        _items.AddLast(item);
                }
            }
            catch
            {
                // Bozuk log dosyası akışı bozmasın
            }
        }

        private void PersistAllUnsafe()
        {
            try
            {
                var json = JsonSerializer.Serialize(_items, _jsonOpts);
                File.WriteAllText(StorageFilePath, json);
            }
            catch
            {
                // Disk hatası log akışını bozmasın
            }
        }

        private static string ResolveStoragePath(string? storageFilePath)
        {
            if (!string.IsNullOrWhiteSpace(storageFilePath))
                return storageFilePath;

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appData, "SputterNewApp", "event-log.json");
        }
    }
}
