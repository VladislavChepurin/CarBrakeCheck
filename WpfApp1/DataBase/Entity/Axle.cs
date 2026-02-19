namespace WpfApp1.DataBase.Entity
{
    public class Axle
    {
        public int Id { get; set; }

        // Внешний ключ к модели автомобиля
        public int CarModelId { get; set; }
        public virtual CarModel CarModel { get; set; }

        /// <summary>
        /// Номер оси (1,2,3...). Оси в рамках одной модели нумеруются по порядку.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Направление вращения: прямое (Forward) или обратное (Reverse)
        /// </summary>
        public RotationDirection RotationDirection { get; set; }

        /// <summary>
        /// Наличие стояночного тормоза на оси
        /// </summary>
        public bool HasParkingBrake { get; set; }

        /// <summary>
        /// Тип тормозов: барабанные (Drum) или дисковые (Disc)
        /// </summary>
        public BrakeType BrakeType { get; set; }

        /// <summary>
        /// Наличие регулятора тормозного усилия
        /// </summary>
        public bool HasRegulator { get; set; }
    }

    public enum RotationDirection
    {
        Forward = 0,
        Reverse = 1
    }

    public enum BrakeType
    {
        Drum = 0,
        Disc = 1
    }
}