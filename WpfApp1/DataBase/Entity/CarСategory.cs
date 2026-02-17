namespace WpfApp1.DataBase.Entity
{
    public class CarСategory
    {
        public int Id { get; set; }
        public string? СategoryName { get; set; }

        // Коллекция моделей для обратной навигации — virtual + инициализация
        public virtual ICollection<CarModel> CarModels { get; set; } = new HashSet<CarModel>();
    }
}
