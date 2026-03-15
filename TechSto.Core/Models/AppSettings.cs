using System.Text.Json;

namespace TechSto.Core.Models
{
    public class AppSettings
    {
        public string Language { get; set; } = "ru-RU";
        public string? LastSelectedComPort { get; set; }
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;

        private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TechSto", "settings.json");

        // Кешируем JsonSerializerOptions
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string? directory = Path.GetDirectoryName(SettingsPath);
                if (directory is not null)
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    string json = JsonSerializer.Serialize(this, JsonOptions);
                    File.WriteAllText(SettingsPath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}