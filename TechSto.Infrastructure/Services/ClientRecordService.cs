using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TechSto.Core.DTOs;
using TechSto.Core.Interfaces;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class ClientRecordService : IClientRecordService
    {
        private readonly MainContext _context;

        public ClientRecordService(MainContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Загружает все записи для главной таблицы с данными о владельце, автомобиле, модели, марке и дате последней проверки.
        /// </summary>
        public List<ClientRecordDto> LoadClientRecords()
        {
            var records = _context.TheCars!
            .Include(c => c.Owner)
            .Include(c => c.CarModel).ThenInclude(m => m!.CarBrand)
            .Include(c => c.CarModel).ThenInclude(m => m!.CarСategory) // для категории
            .Include(c => c.CarModel).ThenInclude(m => m!.Axles)       // для подсчёта осей
            .Select(c => new ClientRecordDto
            {
                CarId = c.Id,
                OwnerName = c.Owner != null ? c.Owner.Name : null,
                OwnerSurname = c.Owner != null ? c.Owner.Surname : null,
                GosNumber = c.GosNumber,
                VinCode = c.VinCode,
                BrandName = c.CarModel != null && c.CarModel.CarBrand != null
                    ? c.CarModel.CarBrand.BrandName : null,
                Model = c.CarModel != null ? c.CarModel.ModelName : null,
                CategoryName = c.CarModel != null && c.CarModel.CarСategory != null
                    ? c.CarModel.CarСategory.CategoryName : null,
                AxlesCount = c.CarModel != null ? c.CarModel.Axles.Count : 0,                
                CurbMass = c.CarModel!.CurbMass ?? 0,
                MaxMass = c.CarModel.MaxMass ?? 0,
                LastTestDateString = c.DataChecks
                    .OrderByDescending(d => d.Data)
                    .Select(d => d.Data)
                    .FirstOrDefault()
            })
            .ToList();

            // Преобразование строки в DateTime
            foreach (var record in records)
            {
                if (record.LastTestDateString != null &&
                    DateTime.TryParseExact(record.LastTestDateString, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    record.LastTestDate = date;
                }
            }

            return records;
        }

        public void DeleteClientRecord(int carId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Загружаем автомобиль с владельцем и проверками
                var car = _context.TheCars
                    .Include(c => c.Owner)
                    .Include(c => c.DataChecks)
                    .FirstOrDefault(c => c.Id == carId);

                if (car == null)
                    throw new InvalidOperationException($"Автомобиль с ID {carId} не найден.");

                // Удаляем все проверки, связанные с автомобилем
                if (car.DataChecks != null && car.DataChecks.Any())
                {
                    _context.Checks?.RemoveRange(car.DataChecks);
                }

                // Удаляем сам автомобиль
                _context.TheCars.Remove(car);

                // Если у владельца нет других автомобилей, удаляем и его
                if (car.Owner != null)
                {
                    var hasOtherCars = _context.TheCars.Any(c => c.OwnerId == car.Owner.Id && c.Id != carId);
                    if (!hasOtherCars)
                    {
                        _context.Owners.Remove(car.Owner);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
