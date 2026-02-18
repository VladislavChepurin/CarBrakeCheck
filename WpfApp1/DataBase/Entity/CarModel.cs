namespace WpfApp1.DataBase.Entity
{
    public class CarModel
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }

        // Внешние ключи (может быть null, если связь необязательна)
        public int? CarBrandId { get; set; }
        public int? CarCategoryId { get; set; }

        // Навигационные свойства — обязательно virtual
        public virtual CarBrand? CarBrand { get; set; }
        public virtual CarСategory? CarСategory { get; set; }

         // Навигационное свойство – список автомобилей этой модели (опционально)
        public virtual ICollection<TheCar> TheCars { get; set; } = new HashSet<TheCar>();
    }  
}
