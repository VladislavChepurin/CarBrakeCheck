using System.ComponentModel;
using System.Runtime.CompilerServices;
using TechSto.DataBase.Entity;

namespace TechSto.ViewModels
{

    public class CarBrandViewModel : INotifyPropertyChanged
    {
        private readonly CarBrand _model;

        public CarBrandViewModel(CarBrand model)
        {
            _model = model;
        }

        public CarBrand Model => _model; // доступ к оригинальной модели

        public int Id => _model.Id;

        public string BrandName
        {
            get => _model.BrandName!;
            set
            {
                if (_model.BrandName != value)
                {
                    _model.BrandName = value;
                    OnPropertyChanged();
                }
            }
        }

        // Можно добавить другие свойства

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}