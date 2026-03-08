using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TechSto.Core.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Surname { get; set; }

        // Навигационное свойство: список автомобилей владельца
        public virtual ICollection<TheCar> TheCars { get; set; } = new ObservableCollection<TheCar>();   
    }
}
