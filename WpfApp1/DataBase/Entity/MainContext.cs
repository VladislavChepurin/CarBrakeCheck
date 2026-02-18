using Microsoft.EntityFrameworkCore;

namespace WpfApp1.DataBase.Entity
{
    public class MainContext : DbContext
    {      
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarСategory> CarСategories { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<TheCar> TheCars { get; set; }
        public DbSet<DataCheck> DataChecks { get; set; }


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

            // Owner -> DataChecks
            modelBuilder.Entity<DataCheck>()
                .HasOne(d => d.Owner)
                .WithMany(o => o.DataChecks)
                .HasForeignKey(d => d.OwnerId)
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
        }
    }
}
