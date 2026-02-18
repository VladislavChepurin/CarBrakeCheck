namespace WpfApp1.DataBase.Entity
{
    public class TheCar
    {
        public int Id { get; set; }
        public string GosNumber { get; set; }           // госномер
        public string VinCode { get; set; }             // VIN-код

        // Внешний ключ к модели автомобиля (CarModel)
        public int CarModelId { get; set; }
        public virtual CarModel CarModel { get; set; }   // навигационное свойство

        // Внешний ключ к владельцу
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }         // навигационное свойство
    }
}