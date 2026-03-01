using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TechSto.WPF.DataBase.Entity;

namespace TechSto.WPF.BusinessLayer
{
    public class OwnerService
    {
        private readonly MainContext _context;

        public OwnerService(MainContext context)
        {
            _context = context;
        }

        // ---------- Чтение данных ----------

        /// <summary>
        /// Получить всех владельцев с возможностью загрузки связанных данных (автомобили и проверки)
        /// </summary>
        public IQueryable<Owner> GetAll(bool includeCars = true, bool includeChecks = true)
        {
            var query = _context.Owners.AsQueryable();
            if (includeCars)
                query = query.Include(o => o.TheCars);           
            return query;
        }

        public Owner? GetById(int id)
        {
            return GetById(id, _context);
        }

        /// <summary>
        /// Получить владельца по Id с загрузкой автомобилей и проверок
        /// </summary>
        public Owner? GetById(int id, MainContext _context)
        {
            return _context.Owners
                .Include(o => o.TheCars)
                .FirstOrDefault(o => o.Id == id);
        }

        /// <summary>
        /// Загрузить всех владельцев в локальный кэш и вернуть ObservableCollection для привязки (WPF)
        /// </summary>
        public ObservableCollection<Owner> GetLocalOwners()
        {
            // Принудительно загружаем связанные данные, чтобы они были доступны в локальном кэше
            _context.Owners
                .Include(o => o.TheCars)
                .Load();
            return _context.Owners.Local.ToObservableCollection();
        }

        // ---------- Создание ----------

        /// <summary>
        /// Добавить нового владельца
        /// </summary>
        public void Add(Owner owner)
        {
            _context.Owners.Add(owner);
            _context.SaveChanges();
        }

        /// <summary>
        /// Создать владельца с именем (автомобили и проверки можно добавить позже)
        /// </summary>
        public Owner CreateOwner(string name)
        {
            var owner = new Owner { Name = name };
            _context.Owners.Add(owner);
            _context.SaveChanges();
            return owner;
        }

        // ---------- Обновление ----------

        /// <summary>
        /// Обновить основные данные владельца (без синхронизации коллекций автомобилей и проверок)
        /// </summary>
        public void Update(Owner owner)
        {
            var existing = _context.Owners.Find(owner.Id);
            if (existing == null)
                throw new InvalidOperationException("Владелец не найден.");

            _context.Entry(existing).CurrentValues.SetValues(owner);
            _context.SaveChanges();
        }

        /// <summary>
        /// Полное обновление владельца с синхронизацией коллекций TheCars и DataChecks
        /// </summary>
        public void UpdateFull(Owner updatedOwner)
        {
            var existing = _context.Owners
                .Include(o => o.TheCars)                
                .FirstOrDefault(o => o.Id == updatedOwner.Id);
            if (existing == null)
                throw new InvalidOperationException("Владелец не найден.");

            // Обновляем скалярные свойства
            _context.Entry(existing).CurrentValues.SetValues(updatedOwner);

            // Синхронизация автомобилей (TheCars)
            // Удаляем автомобили, которых нет в обновлённой версии
            foreach (var car in existing.TheCars.ToList())
            {
                if (!updatedOwner.TheCars.Any(c => c.Id == car.Id))
                    _context.TheCars.Remove(car);
            }

            // Обновляем существующие и добавляем новые автомобили
            foreach (var updatedCar in updatedOwner.TheCars)
            {
                if (updatedCar.Id == 0)
                {
                    // Новый автомобиль
                    updatedCar.OwnerId = existing.Id;
                    _context.TheCars.Add(updatedCar);
                }
                else
                {
                    var existingCar = existing.TheCars.FirstOrDefault(c => c.Id == updatedCar.Id);
                    if (existingCar != null)
                        _context.Entry(existingCar).CurrentValues.SetValues(updatedCar);
                }
            }         

            _context.SaveChanges();
        }

        // ---------- Удаление ----------

        /// <summary>
        /// Удалить владельца по Id (связанные автомобили и проверки удалятся каскадно, если настроено в БД)
        /// </summary>
        public void Delete(int id)
        {
            var owner = _context.Owners.Find(id);
            if (owner == null) return;

            _context.Owners.Remove(owner);
            _context.SaveChanges();
        }

        // ---------- Работа с дочерними сущностями (если нужно отдельно) ----------

        /// <summary>
        /// Добавить автомобиль существующему владельцу
        /// </summary>
        public void AddCar(int ownerId, TheCar car)
        {
            var owner = _context.Owners.Find(ownerId);
            if (owner == null)
                throw new InvalidOperationException("Владелец не найден.");

            car.OwnerId = ownerId;
            _context.TheCars.Add(car);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удалить автомобиль
        /// </summary>
        public void DeleteCar(int carId)
        {
            var car = _context.TheCars.Find(carId);
            if (car != null)
            {
                _context.TheCars.Remove(car);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Добавить проверку существующему владельцу
        /// </summary>
        public void AddDataCheck(int ownerId, Check check)
        {
            var owner = _context.Owners.Find(ownerId);
            if (owner == null)
                throw new InvalidOperationException("Владелец не найден.");
                     
            _context.Checks.Add(check);
            _context.SaveChanges();
        }

        /// <summary>
        /// Удалить проверку
        /// </summary>
        public void DeleteDataCheck(int checkId)
        {
            var check = _context.Checks.Find(checkId);
            if (check != null)
            {
                _context.Checks.Remove(check);
                _context.SaveChanges();
            }
        }

        // ---------- Сохранение (если нужно выполнить несколько операций подряд) ----------

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}