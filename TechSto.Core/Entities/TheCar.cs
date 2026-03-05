using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TechSto.Core.Entities
{
    public class TheCar
    {
        public int Id { get; set; }

        [Required]
        public string GosNumber { get; set; }           // госномер
        public string VinCode { get; set; }          // VIN-код
        public string FrameNumber { get; set; }         // Номер рамы

        [Required]
        // Внешний ключ к модели автомобиля (CarModel)
        public int CarModelId { get; set; }
        public virtual CarModel CarModel { get; set; }   // навигационное свойство

        [Required]
        // Внешний ключ к владельцу
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }         // навигационное свойство

        // Навигационное свойство: список проверок автомобиля
        public virtual ICollection<Check> DataChecks { get; set; } = new ObservableCollection<Check>();

    }
}