using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TechSto.Core.DTOs;
using TechSto.Core.Entities;
using TechSto.Core.Interfaces;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class CheckService : ICheckService
    {
        private readonly MainContext _context;

        public CheckService(MainContext context)
        {
            _context = context;
        }

        public Check AddCheck(int carId, int carMileage, CheckResultType? checkResult = null)
        {
            // Проверяем, существует ли автомобиль с таким ID
            var car = _context.TheCars.Find(carId);
            if (car == null)
                throw new ArgumentException($"Автомобиль с ID {carId} не найден", nameof(carId));

            var check = new Check
            {
                TheCarId = carId,
                CarMileage = carMileage,
                CheckResult = checkResult,
                Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.Checks.Add(check);
            _context.SaveChanges();

            return check;
        }

        public bool DeleteCheck(int checkId)
        {
            var check = _context.Checks.Find(checkId);
            if (check == null)
                return false;

            _context.Checks.Remove(check);
            _context.SaveChanges();
            return true;
        }

        public int DeleteAllChecksByCarId(int carId)
        {
            var checks = _context.Checks.Where(c => c.TheCarId == carId);
            int count = checks.Count();

            if (count > 0)
            {
                _context.Checks.RemoveRange(checks);
                _context.SaveChanges();
            }

            return count;
        }
       
        public List<Check> GetChecksByCarId(int carId)
        {
            return _context.Checks
                .Where(c => c.TheCarId == carId)
                .OrderByDescending(c => c.DataDateTime) // Сортировка по дате (сначала новые)
                .ToList();
        }

        public List<CheckDto> GetChecksDtoByCarId(int carId)
        {
            var checks = _context.Checks
             .Where(c => c.TheCarId == carId)
             .OrderBy(c => c.Data) // Сортировка по строковому полю Data
             .Select(c => new
             {
                 c.Id,
                 DataString = c.Data,
                 c.CheckResult
             })
             .ToList();

            return checks.Select((c, index) => new CheckDto
            {
                Id = c.Id,
                Number = index + 1,
                Date = DateTime.ParseExact(c.DataString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                IsPassed = c.CheckResult == CheckResultType.Yes
            }).ToList();
        }

        public Check? GetCheckById(int checkId)
        {
            return _context.Checks.Find(checkId);
        }      
    }
}