using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using WpfApp1.DataBase.Entity;

namespace WpfApp1.BusinessLayer
{
    public class CarBrandService
    {
        private readonly MainContext _context;

        public CarBrandService(MainContext context)
        {
            _context = context;
        }

        // Получить все марки (с отслеживанием или без)
        public List<CarBrand> GetAll()
        {
            return _context.CarBrands.ToList();
        }

        // Получить марку по Id
        public CarBrand GetById(int id)
        {
            return _context.CarBrands.Find(id);
        }

        // Загрузить все марки в локальный кэш и вернуть ObservableCollection для привязки
        public ObservableCollection<CarBrand> GetLocalBrands()
        {
            _context.CarBrands.Load();   // загружаем данные в локальный кэш
            return _context.CarBrands.Local.ToObservableCollection();
        }

        // Добавить новую марку
        public void Add(CarBrand brand)
        {
            _context.CarBrands.Add(brand);
            _context.SaveChanges();
        }

        // Обновить существующую марку
        public void Update(CarBrand brand)
        {
            _context.Entry(brand).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Удалить марку по Id
        public void Delete(int id)
        {
            var brand = _context.CarBrands.Find(id);
            if (brand != null)
            {
                _context.CarBrands.Remove(brand);
                _context.SaveChanges();
            }
        }

        // Сохранить изменения (если нужно вызвать отдельно)
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
