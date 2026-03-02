using TechSto.Infrastructure.Data;

namespace TechSto.WPF.ViewModels
{
    public class ClientViewModel: ViewModelBase
    {
        private readonly MainContext _context;

        public ClientViewModel(MainContext context)
        {
            _context = context;
        }               
    }
}
