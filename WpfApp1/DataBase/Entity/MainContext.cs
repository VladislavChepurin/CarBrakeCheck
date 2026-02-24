using Microsoft.EntityFrameworkCore;

namespace TechSto.DataBase.Entity
{
    public class MainContext : DbContext
    {      
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarСategory> CarСategories { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<TheCar> TheCars { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Axle> Axles { get; set; }

        // Настройка подключения и параметров
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite("Data Source=DataBase/DataBase.db");
            optionsBuilder.UseLazyLoadingProxies(); // обязательно для ленивой загрузки
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Owner -> TheCars
            modelBuilder.Entity<TheCar>()
                .HasOne(t => t.Owner)
                .WithMany(o => o.TheCars)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // TheCar -> Checks
            modelBuilder.Entity<Check>()
                .HasOne(d => d.TheCar)
                .WithMany(o => o.DataChecks)
                .HasForeignKey(d => d.TheCarId)
                .OnDelete(DeleteBehavior.Cascade);

            // TheCar -> CarModel (многие к одному)
            modelBuilder.Entity<TheCar>()
                .HasOne(t => t.CarModel)
                .WithMany(m => m.TheCars)
                .HasForeignKey(t => t.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            // CarModel -> CarBrand (многие к одному)
            modelBuilder.Entity<CarModel>()
                .HasOne(m => m.CarBrand)
                .WithMany(b => b.CarModels)
                .HasForeignKey(m => m.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // CarModel -> CarCategory (многие к одному)
            modelBuilder.Entity<CarModel>()
                .HasOne(m => m.CarСategory)
                .WithMany(c => c.CarModels)
                .HasForeignKey(m => m.CarCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Новая настройка для Axle
            modelBuilder.Entity<Axle>()
                .HasOne(a => a.CarModel)
                .WithMany(m => m.Axles)
                .HasForeignKey(a => a.CarModelId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении модели удаляются и её оси

            // Опционально: добавить уникальный индекс на пару (CarModelId, Order),
            // чтобы в рамках одной модели номера осей не повторялись.
            modelBuilder.Entity<Axle>()
                .HasIndex(a => new { a.CarModelId, a.Order })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
