using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TechSto.Core.Entities;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class TheCarService
    {
        private readonly MainContext _context;

        public TheCarService(MainContext context)
        {
            _context = context;
        }

        // ---------- Чтение данных ----------

        /// <summary>
        /// Получить все автомобили с загрузкой связанных данных (модель, владелец)
        /// </summary>
        public IQueryable<TheCar> GetAll(bool includeModel = true, bool includeOwner = true)
        {
            var query = _context.TheCars.AsQueryable();
            if (includeModel)
                query = query.Include(c => c.CarModel)
                             .ThenInclude(m => m.CarBrand)
                             .Include(c => c.CarModel)
                             .ThenInclude(m => m.CarСategory);
            if (includeOwner)
                query = query.Include(c => c.Owner);
            return query;
        }

        /// <summary>
        /// Получить автомобиль по Id с загрузкой связанных данных
        /// </summary>
        public TheCar GetById(int id)
        {
            return _context.TheCars
                .Include(c => c.CarModel)
                .ThenInclude(m => m.CarBrand)
                .Include(c => c.CarModel)
                .ThenInclude(m => m.CarСategory)
                .Include(c => c.Owner)
                .Include(c => c.DataChecks)
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Получить автомобили для конкретного владельца
        /// </summary>
        public IQueryable<TheCar> GetByOwner(int ownerId)
        {
            return _context.TheCars
                .Include(c => c.CarModel)
                .Where(c => c.OwnerId == ownerId);
        }

        /// <summary>
        /// Загрузить все автомобили в локальный кэш и вернуть ObservableCollection для привязки (WPF)
        /// </summary>
        public ObservableCollection<TheCar> GetLocalCars()
        {
            _context.TheCars
                .Include(c => c.CarModel)
                .Include(c => c.Owner)
                .Include(c => c.DataChecks)
                .Load();
            return _context.TheCars.Local.ToObservableCollection();
        }

        // ---------- Создание ----------

        /// <summary>
        /// Добавить новый автомобиль
        /// </summary>
        public void Add(TheCar car)
        {
            _context.TheCars.Add(car);
            _context.SaveChanges();
        }

        /// <summary>
        /// Создать автомобиль с указанными параметрами
        /// </summary>
        public TheCar CreateCar(string gosNumber, string bodyNumber, string frameNumber, int carModelId, int ownerId)
        {
            var model = _context.CarModels.Find(carModelId);
            if (model == null)
                throw new InvalidOperationException("Модель автомобиля не найдена.");
            var owner = _context.Owners.Find(ownerId);
            if (owner == null)
                throw new InvalidOperationException("Владелец не найден.");

            var car = new TheCar
            {
                GosNumber = gosNumber,
                VinCode = bodyNumber,
                FrameNumber = frameNumber,
                CarModelId = carModelId,
                OwnerId = ownerId
            };
            _context.TheCars.Add(car);
            _context.SaveChanges();
            return car;
        }

        // ---------- Обновление ----------

        /// <summary>
        /// Обновить данные автомобиля
        /// </summary>
        public void Update(TheCar updatedCar)
        {
            var existing = _context.TheCars.Find(updatedCar.Id);
            if (existing == null)
                throw new InvalidOperationException("Автомобиль не найден.");

            existing.GosNumber = updatedCar.GosNumber;
            existing.VinCode = updatedCar.VinCode;
            existing.FrameNumber = updatedCar.FrameNumber;
            existing.CarModelId = updatedCar.CarModelId;
            existing.OwnerId = updatedCar.OwnerId;

            _context.SaveChanges();
        }

        /// <summary>
        /// Частичное обновление через делегат
        /// </summary>
        public void UpdatePartial(int carId, Action<TheCar> updateAction)
        {
            var car = _context.TheCars.Find(carId);
            if (car == null)
                throw new InvalidOperationException("Автомобиль не найден.");
            updateAction(car);
            _context.SaveChanges();
        }

        // ---------- Удаление ----------

        /// <summary>
        /// Удалить автомобиль по Id
        /// </summary>
        public void Delete(int id)
        {
            var car = _context.TheCars.Find(id);
            if (car == null) return;
            _context.TheCars.Remove(car);
            _context.SaveChanges();
        }

        // ---------- Дополнительные методы ----------

        /// <summary>
        /// Получить все модели автомобилей для выпадающего списка
        /// </summary>
        public List<CarModel> GetAllModels()
        {
            return _context.CarModels.ToList();
        }

        /// <summary>
        /// Получить всех владельцев для выпадающего списка
        /// </summary>
        public List<Owner> GetAllOwners()
        {
            return _context.Owners.ToList();
        }

        /// <summary>
        /// Проверить уникальность госномера (если требуется)
        /// </summary>
        public bool IsGosNumberUnique(string gosNumber, int? excludeCarId = null)
        {
            var query = _context.TheCars.Where(c => c.GosNumber == gosNumber);
            if (excludeCarId.HasValue)
                query = query.Where(c => c.Id != excludeCarId.Value);
            return !query.Any();
        }

        // ---------- Сохранение ----------

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}