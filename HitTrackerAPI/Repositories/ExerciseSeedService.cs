using System.Text.Json;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class ExerciseSeedService
    {
        private readonly AppDbContext _db;

        private static readonly Dictionary<string, string> MuscleMap = new()
        {
            { "chest", "Chest" }, { "triceps", "Triceps" }, { "biceps", "Biceps" },
            { "shoulders", "Shoulders" }, { "middle back", "Back" }, { "upper back", "Back" },
            { "lower back", "Back" }, { "lats", "Back" }, { "traps", "Traps" },
            { "forearms", "Forearms" }, { "quadriceps", "Quadriceps" },
            { "hamstrings", "Hamstrings" }, { "glutes", "Glutes" }, { "calves", "Calves" },
            { "abductors", "Legs" }, { "adductors", "Legs" }, { "abdominals", "Abs" },
            { "neck", "Neck" }
        };

        private static readonly Dictionary<string, string> EquipmentMap = new()
        {
            { "barbell", "Barbell" }, { "dumbbell", "Dumbbell" },
            { "body only", "Bodyweight" }, { "machine", "Machine" },
            { "cable", "Cable" }, { "kettlebells", "Kettlebell" },
            { "bands", "Resistance Band" }, { "e-z curl bar", "EZ Bar" },
            { "other", "Other" }
        };

        private static readonly Dictionary<string, int> RestDaysMap = new()
        {
            { "Chest", 5 }, { "Back", 6 }, { "Quadriceps", 7 },
            { "Hamstrings", 7 }, { "Glutes", 6 }, { "Shoulders", 5 },
            { "Triceps", 4 }, { "Biceps", 4 }, { "Calves", 3 },
            { "Abs", 3 }, { "Traps", 4 }, { "Forearms", 3 },
            { "Legs", 7 }, { "Neck", 3 }
        };

        public ExerciseSeedService(AppDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync(string dataDir)
        {
            if (_db.Exercises.Any(e => !e.IsCustom)) return;

            if (!Directory.Exists(dataDir))
            {
                Console.WriteLine($"exercises_data not found: {dataDir}");
                return;
            }

            var folders = Directory.GetDirectories(dataDir);
            int saved = 0, skipped = 0;

            foreach (var folder in folders)
            {
                var jsonPath = Path.Combine(folder, "exercise.json");
                if (!File.Exists(jsonPath)) { skipped++; continue; }

                try
                {
                    var json = await File.ReadAllTextAsync(jsonPath);
                    var data = JsonDocument.Parse(json).RootElement;

                    var level = data.TryGetProperty("level", out var lvl) ? lvl.GetString() : "";
                    if (level != "beginner" && level != "intermediate") { skipped++; continue; }

                    var name = data.TryGetProperty("name", out var n) ? n.GetString()?.Trim() : null;
                    if (string.IsNullOrEmpty(name)) { skipped++; continue; }

                    var primaryMuscle = data.TryGetProperty("primaryMuscles", out var pm)
                        && pm.GetArrayLength() > 0
                        ? pm[0].GetString()?.ToLower() : "";
                    var muscleGroup = MuscleMap.GetValueOrDefault(primaryMuscle ?? "", "General");

                    var eq = data.TryGetProperty("equipment", out var eqProp)
                        ? eqProp.GetString()?.ToLower() : "";
                    var equipment = EquipmentMap.GetValueOrDefault(eq ?? "", "Other");

                    var restDays = RestDaysMap.GetValueOrDefault(muscleGroup, 5);

                    string? imageUrl = null, imageUrl2 = null;
                    var imagesDir = Path.Combine(folder, "images");
                    if (Directory.Exists(imagesDir))
                    {
                        var folderName = Path.GetFileName(folder);
                        var images = Directory.GetFiles(imagesDir)
                            .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".gif"))
                            .OrderBy(f => f)
                            .ToList();

                        if (images.Count > 0)
                            imageUrl = $"/{folderName}/images/{Path.GetFileName(images[0])}";
                        if (images.Count > 1)
                            imageUrl2 = $"/{folderName}/images/{Path.GetFileName(images[1])}";
                    }

                    var description = data.TryGetProperty("instructions", out var inst)
                        ? string.Join(" ", inst.EnumerateArray().Select(i => i.GetString()))
                        : null;
                    if (description?.Length > 1000)
                        description = description[..1000];

                    // Instructions pipe ile ayır
                    var instructions = data.TryGetProperty("instructions", out var instArr)
                        ? string.Join("|", instArr.EnumerateArray().Select(i => i.GetString()))
                        : null;

                    // Secondary muscles virgül ile ayır
                    var secondaryMuscles = data.TryGetProperty("secondaryMuscles", out var secArr)
                        ? string.Join(",", secArr.EnumerateArray().Select(i => i.GetString()))
                        : null;

                    var exercise = new Exercise
                    {
                        Name             = name,
                        MuscleGroup      = muscleGroup,
                        Description      = description,
                        RequiredRestDays = restDays,
                        Equipment        = equipment,
                        IsCustom         = false,
                        Level            = level,
                        ImageUrl         = imageUrl,
                        ImageUrl2        = imageUrl2,
                        Instructions     = instructions,
                        SecondaryMuscles = secondaryMuscles,
                        UserId           = null
                    };

                    _db.Exercises.Add(exercise);
                    saved++;

                    if (saved % 50 == 0)
                    {
                        await _db.SaveChangesAsync();
                        Console.WriteLine($"  ✅ {saved} saved...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    skipped++;
                }
            }

            await _db.SaveChangesAsync();
            Console.WriteLine($"\n🎉 Seed complete! Saved: {saved}, Skipped: {skipped}");
        }
    }
}