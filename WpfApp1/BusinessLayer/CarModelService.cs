using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using WpfApp1.DataBase.Entity;

namespace WpfApp1.BusinessLayer
{
    public class CarModelService
    {
        private readonly MainContext _context;

        public CarModelService(MainContext context)
        {
            _context = context;
        }

        // ---------- Чтение данных ----------

        /// <summary>
        /// Получить все модели с загрузкой связанных осей (опционально)
        /// </summary>
        public IQueryable<CarModel> GetAll(bool includeAxles = true)
        {
            if (includeAxles)
                return _context.CarModels.Include(m => m.Axles);
            else
                return _context.CarModels;
        }

        /// <summary>
        /// Получить модель по Id с загрузкой осей
        /// </summary>
        public CarModel GetById(int id)
        {
            return _context.CarModels
                .Include(m => m.Axles)
                .FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Загрузить все модели в локальный кэш и вернуть ObservableCollection для привязки
        /// </summary>
        public ObservableCollection<CarModel> GetLocalModels()
        {
            _context.CarModels.Include(m => m.Axles).Load();
            return _context.CarModels.Local.ToObservableCollection();
        }

        /// <summary>
        /// Получить модели для указанного бренда с возможностью загрузки осей
        /// </summary>
        /// <param name="brandId">Идентификатор бренда</param>
        /// <param name="includeAxles">Загружать ли связанные оси</param>
        public IQueryable<CarModel> GetByBrand(int brandId, bool includeAxles = true)
        {
            var query = _context.CarModels.Where(m => m.CarBrandId == brandId);
            if (includeAxles)
                query = query.Include(m => m.Axles);
            return query;
        }

        /// <summary>
        /// Загрузить модели для указанного бренда в локальный кэш и вернуть ObservableCollection для привязки (WPF)
        /// </summary>
        /// <param name="brandId">Идентификатор бренда</param>
        public ObservableCollection<CarModel> GetLocalModelsByBrand(int brandId)
        {
            _context.CarModels
                .Where(m => m.CarBrandId == brandId)
                .Include(m => m.Axles)
                .Load();
            return _context.CarModels.Local.ToObservableCollection();
        }

        // ---------- Создание ----------

        /// <summary>
        /// Создать новую модель с указанным количеством осей (по умолчанию 2)
        /// </summary>
        public CarModel CreateModel(string modelName, int? brandId, int? categoryId,
                                     int axleCount = 2,
                                     int? maxMass = null,
                                     int? curbMass = null,
                                     int? brakeForceDifference = null,
                                     ParkingBrakeType? parkingBrake = null,
                                     ReserveBrakeSystem? reserveBrake = null)
        {
            if (axleCount < 1 || axleCount > 6)
                throw new ArgumentOutOfRangeException(nameof(axleCount), "Количество осей должно быть от 1 до 6.");

            var model = new CarModel
            {
                ModelName = modelName,
                CarBrandId = brandId,
                CarCategoryId = categoryId,
                MaxMass = maxMass,
                CurbMass = curbMass,
                BrakeForceDifference = brakeForceDifference,
                ParkingBrake = parkingBrake,
                ReserveBrake = reserveBrake
            };

            // Создаём оси с параметрами по умолчанию
            for (int i = 1; i <= axleCount; i++)
            {
                model.Axles.Add(new Axle
                {
                    Order = i,
                    RotationDirection = RotationDirection.Forward,
                    HasParkingBrake = false,
                    BrakeType = BrakeType.Disc,
                    HasRegulator = false
                });
            }

            _context.CarModels.Add(model);
            _context.SaveChanges();
            return model;
        }

        // ---------- Обновление ----------

        /// <summary>
        /// Обновить основные поля модели и синхронизировать коллекцию осей.
        /// </summary>
        /// <param name="updatedModel">Обновлённая модель с заполненной коллекцией Axles</param>
        public void Update(CarModel updatedModel)
        {
            // Проверяем, что модель существует
            var existingModel = _context.CarModels
                .Include(m => m.Axles)
                .FirstOrDefault(m => m.Id == updatedModel.Id);

            if (existingModel == null)
                throw new InvalidOperationException("Модель не найдена.");

            // Обновляем скалярные свойства
            _context.Entry(existingModel).CurrentValues.SetValues(updatedModel);

            // Синхронизация коллекции осей
            // Удаляем те оси, которых нет в обновлённой модели
            foreach (var existingAxle in existingModel.Axles.ToList())
            {
                if (!updatedModel.Axles.Any(a => a.Id == existingAxle.Id))
                    _context.Axles.Remove(existingAxle);
            }

            // Обновляем существующие оси и добавляем новые
            foreach (var updatedAxle in updatedModel.Axles)
            {
                if (updatedAxle.Id == 0)
                {
                    // Новая ось
                    existingModel.Axles.Add(new Axle
                    {
                        Order = updatedAxle.Order,
                        RotationDirection = updatedAxle.RotationDirection,
                        HasParkingBrake = updatedAxle.HasParkingBrake,
                        BrakeType = updatedAxle.BrakeType,
                        HasRegulator = updatedAxle.HasRegulator
                    });
                }
                else
                {
                    // Существующая ось
                    var existingAxle = existingModel.Axles.FirstOrDefault(a => a.Id == updatedAxle.Id);
                    if (existingAxle != null)
                    {
                        _context.Entry(existingAxle).CurrentValues.SetValues(updatedAxle);
                    }
                }
            }

            _context.SaveChanges();
        }

        // ---------- Удаление ----------

        /// <summary>
        /// Удалить модель (при наличии каскадного удаления оси удалятся автоматически)
        /// </summary>
        public void Delete(int id)
        {
            var model = _context.CarModels.Find(id);
            if (model == null) return;

            // Если настроено каскадное удаление, оси удалятся сами
            _context.CarModels.Remove(model);
            _context.SaveChanges();
        }

        // ---------- Дополнительные методы для работы с осями (если нужно) ----------

        /// <summary>
        /// Добавить ось к существующей модели
        /// </summary>
        public void AddAxle(int carModelId, Axle axle)
        {
            var model = _context.CarModels.Find(carModelId);
            if (model == null) throw new InvalidOperationException("Модель не найдена.");
            axle.CarModelId = carModelId;
            _context.Axles.Add(axle);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удалить ось по Id
        /// </summary>
        public void DeleteAxle(int axleId)
        {
            var axle = _context.Axles.Find(axleId);
            if (axle != null)
            {
                _context.Axles.Remove(axle);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Сохранить все изменения в контексте (если нужно выполнить несколько операций в одной транзакции)
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}