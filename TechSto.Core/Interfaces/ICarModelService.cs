using System.Collections.ObjectModel;
using TechSto.Core.Entities;

namespace TechSto.Core.Interfaces
{
    public interface ICarModelService
    {
        IQueryable<CarModel> GetAll(bool includeAxles = true);
        CarModel GetById(int id);
        ObservableCollection<CarModel> GetLocalModels();
        IQueryable<CarModel> GetByBrand(int brandId, bool includeAxles = true);
        ObservableCollection<CarModel> GetLocalModelsByBrand(int brandId);
        List<CarModel> GetAllWithBrands();
        CarModel AddModelWithAxles(CarModel model);

        CarModel CreateModel(string modelName, int? brandId, int? categoryId,
                                     int axleCount = 2,
                                     int? maxMass = null,
                                     int? curbMass = null,
                                     int? brakeForceDifference = null,
                                     ParkingBrakeType? parkingBrake = null,
                                     ReserveBrakeSystem? reserveBrake = null);
        void Update(CarModel updatedModel);
        void Delete(int id);
        void AddAxle(int carModelId, Axle axle);
        void DeleteAxle(int axleId);
    }
}
