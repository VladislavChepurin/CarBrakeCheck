using System.IO;
using System.Text.Json;

namespace WpfApp1
{
    class AppSettings
    {
        public string Language { get; set; } = "ru-RU";
        public string LastSelectedTab { get; set; } = "Settings";
        public bool IsMaximized { get; set; } = false;
        public double WindowWidth { get; set; } = 1200;
        public double WindowHeight { get; set; } = 650;
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;


        private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WpfApp1", "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}

