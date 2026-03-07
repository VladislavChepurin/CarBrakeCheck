using System.Collections.ObjectModel;
using TechSto.Core.Entities;

namespace TechSto.Core.Interfaces
{
    public interface ITheCarService
    {
        IQueryable<TheCar> GetAll(bool includeModel = true, bool includeOwner = true);
        public TheCar GetById(int id);
        public IQueryable<TheCar> GetByOwner(int ownerId);
        public ObservableCollection<TheCar> GetLocalCars();
        void Add(TheCar car);
        TheCar CreateCar(string gosNumber, string VinCode, int carModelId, int ownerId);
        void Update(TheCar updatedCar);
        void UpdatePartial(int carId, Action<TheCar> updateAction);
        void Delete(int id);
        List<CarModel> GetAllModels();
        List<Owner> GetAllOwners();
        bool IsGosNumberUnique(string gosNumber, int? excludeCarId = null);
    }
}
