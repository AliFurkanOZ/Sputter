using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SputterNewApp.Core.Services;

namespace SputterNewApp.Core.Models
{
    public class MaterialItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public float Density { get; set; }
        public float ZRatio { get; set; }
        public bool IsSeedMaterial { get; set; } = false;
    }

    public class TargetItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Source { get; set; } = "—";
        public decimal UsedWh { get; set; }
    }

    public class ProcessItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string MaterialId { get; set; } = "";
        public string MaterialName { get; set; } = "";
        public float Tooling { get; set; } = 1.0f;
    }

    public enum DepositionSource
    {
        RF1 = 1,
        RF2 = 2,
        DC = 3
    }

    public class LayerItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProcessId { get; set; } = "";
        public float FinalThickness { get; set; } = 0f;
        public DepositionSource Source { get; set; } = DepositionSource.RF1;

        // Kullanıcı seçimi (sensörler)
        public bool RF1SensorEnabled { get; set; } = false;
        public bool RF2SensorEnabled { get; set; } = false;
        public bool DCSensorEnabled { get; set; } = false;
    }

    public static class AppDataManager
    {
        private const string MATERIALS_FILE = "materials.json";
        private const string PROCESSES_FILE = "processes.json";
        private const string LAYERS_FILE = "layers.json";
        private const string TARGETS_FILE = "targets.json";
        private const string CONNECTION_FILE = "connection.json";

        public static string CurrentUserId { get; set; } = "default";

        public static async Task<ConnectionSettings> LoadConnectionSettingsAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, CONNECTION_FILE);
            var fallback = new ConnectionSettings
            {
                IpAddress = "127.0.0.1",
                Port = 502,
                UnitId = 1,
                RegistrationNo = "1234"
            };

            if (!File.Exists(filePath))
            {
                return fallback;
            }

            var loaded = await UserStorage.LoadAsync(filePath, fallback).ConfigureAwait(false);
            return loaded ?? fallback;
        }

        public static async Task SaveConnectionSettingsAsync(ConnectionSettings settings)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, CONNECTION_FILE);
            await UserStorage.SaveAsync(filePath, settings).ConfigureAwait(false);
        }

        public static async Task<List<MaterialItem>> LoadMaterialsAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, MATERIALS_FILE);

            if (!File.Exists(filePath))
            {
                var seedMats = await LoadSeedMaterialsAsync().ConfigureAwait(false);
                if (seedMats.Any())
                {
                    await SaveMaterialsAsync(seedMats).ConfigureAwait(false);
                }
                return seedMats;
            }

            var materials = await UserStorage.LoadAsync(filePath, new List<MaterialItem>()).ConfigureAwait(false);

            if (!materials.Any())
            {
                materials = await LoadSeedMaterialsAsync().ConfigureAwait(false);
                await SaveMaterialsAsync(materials).ConfigureAwait(false);
            }

            return materials;
        }

        public static List<MaterialItem> LoadMaterialsSync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, MATERIALS_FILE);
            if (File.Exists(filePath))
            {
                var list = UserStorage.LoadSync(filePath, new List<MaterialItem>());
                if (list.Any()) return list;
            }
            return new List<MaterialItem>();
        }

        public static async Task<List<MaterialItem>> LoadSeedMaterialsAsync()
        {
            try
            {
                var seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "seed_materials.json");

                if (!File.Exists(seedPath))
                {
                    seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Seed", "seed_materials.json");
                    seedPath = Path.GetFullPath(seedPath);
                }

                if (File.Exists(seedPath))
                {
                    var json = await File.ReadAllTextAsync(seedPath).ConfigureAwait(false);
                    var seedMaterials = JsonSerializer.Deserialize<List<SeedMaterial>>(json);

                    var materials = (seedMaterials ?? new List<SeedMaterial>()).Select(m => new MaterialItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = m.Name,
                        Density = m.Density,
                        ZRatio = m.ZRatio,
                        IsSeedMaterial = true
                    }).ToList();

                    return materials;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed materyalleri yuklenirken hata: {ex.Message}");
            }

            return new List<MaterialItem>();
        }

        public static async Task SaveMaterialsAsync(List<MaterialItem> materials)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, MATERIALS_FILE);
            await UserStorage.SaveAsync(filePath, materials).ConfigureAwait(false);
        }

        public static async Task<List<TargetItem>> LoadTargetsAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, TARGETS_FILE);

            if (!File.Exists(filePath))
            {
                var seedTargets = await LoadSeedTargetsAsync().ConfigureAwait(false);
                if (seedTargets.Any())
                    await SaveTargetsAsync(seedTargets).ConfigureAwait(false);

                return seedTargets;
            }

            if (File.Exists(filePath))
            {
                var targets = await UserStorage.LoadAsync(filePath, new List<TargetItem>()).ConfigureAwait(false);
                if (targets.Any())
                    return targets;
            }

            var fallbackTargets = await LoadSeedTargetsAsync().ConfigureAwait(false);
            if (fallbackTargets.Any())
                await SaveTargetsAsync(fallbackTargets).ConfigureAwait(false);

            return fallbackTargets;
        }

        public static List<TargetItem> LoadTargetsSync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, TARGETS_FILE);
            if (File.Exists(filePath))
            {
                return UserStorage.LoadSync(filePath, new List<TargetItem>());
            }
            return new List<TargetItem>();
        }

        public static async Task SaveTargetsAsync(List<TargetItem> targets)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, TARGETS_FILE);
            await UserStorage.SaveAsync(filePath, targets).ConfigureAwait(false);
        }

        public static async Task<List<TargetItem>> LoadSeedTargetsAsync()
        {
            try
            {
                var seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "seed_targets.json");

                if (!File.Exists(seedPath))
                {
                    seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Seed", "seed_targets.json");
                    seedPath = Path.GetFullPath(seedPath);
                }

                if (File.Exists(seedPath))
                {
                    var json = await File.ReadAllTextAsync(seedPath).ConfigureAwait(false);
                    var seedTargets = JsonSerializer.Deserialize<List<SeedTarget>>(json);

                    return (seedTargets ?? new List<SeedTarget>())
                        .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                        .Select(t => new TargetItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = t.Name.Trim(),
                            Source = "—",
                            UsedWh = t.UsedWh
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed targetlari yuklenirken hata: {ex.Message}");
            }

            return new List<TargetItem>();
        }

        private class SeedMaterial
        {
            public string Name { get; set; } = "";
            public float Density { get; set; }
            public float ZRatio { get; set; }
        }

        private class SeedTarget
        {
            public string Name { get; set; } = "";
            public decimal UsedWh { get; set; }
        }

        public static async Task<List<ProcessItem>> LoadProcessesAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, PROCESSES_FILE);

            if (!File.Exists(filePath))
                return new List<ProcessItem>();

            return await UserStorage.LoadAsync(filePath, new List<ProcessItem>()).ConfigureAwait(false);
        }

        public static List<ProcessItem> LoadProcessesSync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, PROCESSES_FILE);
            if (File.Exists(filePath))
            {
                return UserStorage.LoadSync(filePath, new List<ProcessItem>());
            }
            return new List<ProcessItem>();
        }

        public static async Task SaveProcessesAsync(List<ProcessItem> processes)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, PROCESSES_FILE);
            await UserStorage.SaveAsync(filePath, processes).ConfigureAwait(false);
        }

        public static async Task<List<LayerItem>> LoadLayersAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, LAYERS_FILE);

            if (!File.Exists(filePath))
                return new List<LayerItem>();

            return await UserStorage.LoadAsync(filePath, new List<LayerItem>()).ConfigureAwait(false);
        }

        public static async Task SaveLayersAsync(List<LayerItem> layers)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, LAYERS_FILE);
            await UserStorage.SaveAsync(filePath, layers).ConfigureAwait(false);
        }
    }
}
