using TechSto.DataBase.Entity;

namespace TechSto.BusinessLayer
{
    public class CarCategoryService
    {
        private readonly MainContext _context;

        public CarCategoryService(MainContext context) => _context = context;

        public List<CarСategory> GetAll() => _context.CarСategories.ToList();
    }
}
