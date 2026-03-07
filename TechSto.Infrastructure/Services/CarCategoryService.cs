using TechSto.Core.Entities;
using TechSto.Core.Interfaces;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class CarCategoryService: ICarCategoryService
    {
        private readonly MainContext _context;

        public CarCategoryService(MainContext context) => _context = context;

        public List<CarСategory> GetAll() => _context.CarСategories.ToList();
    }
}
