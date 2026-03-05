using TechSto.Core.Entities;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class CarCategoryService
    {
        private readonly MainContext _context;

        public CarCategoryService(MainContext context) => _context = context;

        public List<CarСategory> GetAll() => _context.CarСategories.ToList();
    }
}
