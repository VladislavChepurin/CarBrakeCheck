using System.Collections.ObjectModel;

namespace WpfApp1.DataBase.Entity
{
    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Навигационное свойство: список автомобилей владельца
        public virtual ICollection<TheCar> TheCars { get; set; } = new ObservableCollection<TheCar>();

        // Навигационное свойство: список проверок владельца
        public virtual ICollection<DataCheck> DataChecks { get; set; } = new ObservableCollection<DataCheck>();
    }
}
