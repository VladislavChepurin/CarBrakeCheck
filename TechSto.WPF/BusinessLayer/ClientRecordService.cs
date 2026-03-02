using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TechSto.Infrastructure.Data;

namespace TechSto.WPF.BusinessLayer
{
    public class ClientRecordService
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
            var records = _context.TheCars
                .Include(c => c.Owner)
                .Include(c => c.CarModel).ThenInclude(m => m!.CarBrand)
                .Select(c => new ClientRecordDto
                {
                    CarId = c.Id,
                    // Если Owner нет, вернём null
                    Owner = c.Owner != null ? c.Owner.Name : null,
                    StateNumber = c.GosNumber,
                    Vin = c.VinСode,
                    // Проверяем всю цепочку: CarModel и CarBrand
                    BrandName = c.CarModel != null && c.CarModel.CarBrand != null
                        ? c.CarModel.CarBrand.BrandName
                        : null,
                    Model = c.CarModel != null ? c.CarModel.ModelName : null,
                    // Безопасно получаем последнюю дату (строка может быть null)
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

    }
}