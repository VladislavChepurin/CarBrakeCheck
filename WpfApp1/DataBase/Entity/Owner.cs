using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TechSto.DataBase.Entity
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string STSNumber { get; set; } //Пока под вопросом необходимость данного поля 

        // Навигационное свойство: список автомобилей владельца
        public virtual ICollection<TheCar> TheCars { get; set; } = new ObservableCollection<TheCar>();   
    }
}
