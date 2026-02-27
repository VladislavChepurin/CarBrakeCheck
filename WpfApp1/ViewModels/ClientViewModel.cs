using TechSto.DataBase.Entity;

namespace TechSto.ViewModels
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
