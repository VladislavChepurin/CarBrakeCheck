using TechSto.Core.Interfaces;
using TechSto.Core.Models;

namespace TechSto.Infrastructure.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public AppSettings Load()
        {
            return AppSettings.Load() ?? new AppSettings();
        }

        public void Save(AppSettings settings)
        {
            settings.Save();
        }
    }
}
