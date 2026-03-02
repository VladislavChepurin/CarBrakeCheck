using System.ComponentModel.DataAnnotations;

namespace TechSto.Core.Entities
{
    public class CarBrand
    {
        public int Id { get; set; }

        [Required]
        public string? BrandName { get; set; }

        // Коллекция моделей для обратной навигации — virtual + инициализация
        public virtual ICollection<CarModel> CarModels { get; set; } = new HashSet<CarModel>();
    }
}
