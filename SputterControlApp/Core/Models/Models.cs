// Models.cs
using System.Text.Json;

namespace SputterControlApp.Core.Models
{
    public class MaterialItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public float Density { get; set; }
        public float ZRatio { get; set; }
        public bool IsSeedMaterial { get; set; } = false; // Seed materyalini işaretlemek için
    }

    public class TargetItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public decimal UsedWh { get; set; }
    }

    public class ProcessItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // Process adı (UI’da görünen)
        public string Name { get; set; } = "";
        // İlişki (tek kaynak)
        public string MaterialId { get; set; } = "";
        // Process parametresi
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

        // ✅ Kullanıcı seçimi (isterse 3ü birden)
        public bool RF1SensorEnabled { get; set; } = false;
        public bool RF2SensorEnabled { get; set; } = false;
        public bool DCSensorEnabled { get; set; } = false;
    }


    // UserStorage sınıfını kullanmak için yeni bir depolama sınıfı
    public static class AppDataManager
    {
        private const string MATERIALS_FILE = "materials.json";
        private const string PROCESSES_FILE = "processes.json";
        private const string LAYERS_FILE = "layers.json";
        private const string TARGETS_FILE = "targets.json";

        // Kullanıcı ID'si - uygulama başlangıcında ayarlanacak
        public static string CurrentUserId { get; set; } = "default";

        // Materyalleri yükle
        public static async Task<List<MaterialItem>> LoadMaterialsAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, MATERIALS_FILE);

            // Eğer dosya yoksa seed materyallerini yükle
            if (!File.Exists(filePath))
            {
                return await LoadSeedMaterialsAsync();
            }

            var materials = await UserStorage.LoadAsync(filePath, new List<MaterialItem>());

            // Eğer materyal listesi boşsa seed materyallerini yükle
            if (!materials.Any())
            {
                materials = await LoadSeedMaterialsAsync();
                await SaveMaterialsAsync(materials);
            }

            return materials;
        }

        // Seed materyallerini yükle
        public static async Task<List<MaterialItem>> LoadSeedMaterialsAsync()
        {
            try
            {
                // Seed dosyasının yolunu bul
                var seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "seed_materials.json");

                if (!File.Exists(seedPath))
                {
                    // Eğer Seed klasöründe yoksa, bir üst dizinde ara
                    seedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Seed", "seed_materials.json");
                    seedPath = Path.GetFullPath(seedPath);
                }

                if (File.Exists(seedPath))
                {
                    var json = await File.ReadAllTextAsync(seedPath);
                    var seedMaterials = JsonSerializer.Deserialize<List<SeedMaterial>>(json);

                    // SeedMaterial'ı MaterialItem'a dönüştür
                    var materials = seedMaterials.Select(m => new MaterialItem
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
                // Log hatası
                Console.WriteLine($"Seed materyalleri yüklenirken hata: {ex.Message}");
            }

            return new List<MaterialItem>();
        }

        // Materyalleri kaydet
        public static async Task SaveMaterialsAsync(List<MaterialItem> materials)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, MATERIALS_FILE);
            await UserStorage.SaveAsync(filePath, materials);
        }

        public static async Task<List<TargetItem>> LoadTargetsAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, TARGETS_FILE);

            if (!File.Exists(filePath))
            {
                var seedTargets = await LoadSeedTargetsAsync();
                if (seedTargets.Any())
                    await SaveTargetsAsync(seedTargets);

                return seedTargets;
            }

            if (File.Exists(filePath))
            {
                var targets = await UserStorage.LoadAsync(filePath, new List<TargetItem>());
                if (targets.Any())
                    return targets;
            }

            var fallbackTargets = await LoadSeedTargetsAsync();
            if (fallbackTargets.Any())
                await SaveTargetsAsync(fallbackTargets);

            return fallbackTargets;
        }

        public static async Task SaveTargetsAsync(List<TargetItem> targets)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, TARGETS_FILE);
            await UserStorage.SaveAsync(filePath, targets);
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
                    var json = await File.ReadAllTextAsync(seedPath);
                    var seedTargets = JsonSerializer.Deserialize<List<SeedTarget>>(json);

                    return (seedTargets ?? new List<SeedTarget>())
                        .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                        .Select(t => new TargetItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = t.Name.Trim(),
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

        // Seed materyal sınıfı
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

            return await UserStorage.LoadAsync(filePath, new List<ProcessItem>());
        }

        public static async Task SaveProcessesAsync(List<ProcessItem> processes)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, PROCESSES_FILE);
            await UserStorage.SaveAsync(filePath, processes);
        }

        public static async Task<List<LayerItem>> LoadLayersAsync()
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, LAYERS_FILE);

            if (!File.Exists(filePath))
                return new List<LayerItem>();

            return await UserStorage.LoadAsync(filePath, new List<LayerItem>());
        }

        public static async Task SaveLayersAsync(List<LayerItem> layers)
        {
            string filePath = UserStorage.GetUserFile(CurrentUserId, LAYERS_FILE);
            await UserStorage.SaveAsync(filePath, layers);
        }


    }
}
