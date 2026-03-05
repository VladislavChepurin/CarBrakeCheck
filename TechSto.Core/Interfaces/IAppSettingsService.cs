using TechSto.Core.Models;

namespace TechSto.Core.Interfaces
{
    public interface IAppSettingsService
    {
        AppSettings Load();

        void Save(AppSettings settings);
    }
}
