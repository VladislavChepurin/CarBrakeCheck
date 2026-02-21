namespace TechSto.DataBase.Entity
{
    public class CarBrand
    {
        public int Id { get; set; }
        public string? BrandName { get; set; }

        // Коллекция моделей для обратной навигации — virtual + инициализация
        public virtual ICollection<CarModel> CarModels { get; set; } = new HashSet<CarModel>();
    }
}
