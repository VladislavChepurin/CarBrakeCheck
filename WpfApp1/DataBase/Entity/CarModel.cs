using System.Collections.Generic;

namespace WpfApp1.DataBase.Entity
{
    public class CarModel
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }

        public int? CarBrandId { get; set; }
        public virtual CarBrand? CarBrand { get; set; }

        public int? CarCategoryId { get; set; }
        public virtual CarСategory? CarСategory { get; set; }

        // Существующие технические характеристики
        public int? MaxMass { get; set; }
        public int? CurbMass { get; set; }
        public int? BrakeForceDifference { get; set; }
        public ParkingBrakeType? ParkingBrake { get; set; }
        public ReserveBrakeSystem? ReserveBrake { get; set; }

        // Связь с автомобилями
        public virtual ICollection<TheCar> TheCars { get; set; } = new HashSet<TheCar>();

        // Новая коллекция осей
        public virtual ICollection<Axle> Axles { get; set; } = new HashSet<Axle>();
    }

    public enum ParkingBrakeType { Foot, Hand }
    public enum ReserveBrakeSystem { None, Hand, Foot }
}