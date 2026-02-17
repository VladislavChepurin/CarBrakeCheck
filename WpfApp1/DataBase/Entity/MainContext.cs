using Microsoft.EntityFrameworkCore;

namespace WpfApp1.DataBase.Entity
{
    public class MainContext : DbContext
    {      
        public DbSet<CarBrand> CarBrands { get; set; }

        public DbSet<CarModel> CarModels { get; set; }

        public DbSet<CarСategory> CarСategories { get; set; }


        // Настройка подключения и параметров
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DataBase/DataBase.db");
            optionsBuilder.UseLazyLoadingProxies(); // обязательно для ленивой загрузки
        }
    }
}
