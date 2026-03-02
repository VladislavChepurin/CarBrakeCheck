using Microsoft.EntityFrameworkCore;
using TechSto.Infrastructure.Data;

namespace TechSto.WPF.BusinessLayer
{
    public class CarDetailsService
    {
        private readonly MainContext _context;

        public CarDetailsService(MainContext context) 
        { 
            _context = context;
        }

        public CarDetailsDto? GetCarDetails(int carId)
        {
            var car = _context.TheCars
                .Include(c => c.Owner)
                .Include(c => c.CarModel)
                    .ThenInclude(m => m.CarBrand)
                .Include(c => c.CarModel)
                    .ThenInclude(m => m.Axles) // загружаем оси
                .FirstOrDefault(c => c.Id == carId);

            if (car == null) return null;

            return new CarDetailsDto
            {
                CarId = car.Id,
                Owner = car.Owner?.Name,
                StateNumber = car.GosNumber,
                Vin = car.VinСode,
                BrandName = car.CarModel?.CarBrand?.BrandName,
                Model = car.CarModel?.ModelName,
                AxlesCount = car.CarModel?.Axles?.Count ?? 0,
                MaxMass = car.CarModel?.MaxMass ?? 0,
                BrakeForceDifference = car.CarModel?.BrakeForceDifference ?? 0,
                // добавьте другие поля по необходимости
            };
        }

    }
}
