using System.Collections.ObjectModel;
using TechSto.Core.Entities;

namespace TechSto.Core.Interfaces
{
    public interface ICarBrandService
    {
        List<CarBrand> GetAll();
        CarBrand GetById(int id);
        ObservableCollection<CarBrand> GetLocalBrands();
        void Add(CarBrand brand);
        void Update(CarBrand brand);
        void Delete(int id);
    }
}
