using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TechSto.DataBase.Entity
{
    public class Check
    {
        public int Id { get; set; }

        [Required]
        public string Data { get; set; }

        public int CarMileage { get; set; }

        public CheckResultType? CheckResult { get; set; }

        [NotMapped]
        public DateTime? DataDateTime
        {
            get => DateTime.TryParseExact(Data, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : (DateTime?)null;
            set => Data = value?.ToString("yyyy-MM-dd HH:mm:ss") ?? throw new ArgumentNullException(nameof(value));
        }

        // Внешний ключ к автомобилю
        public int TheCarId { get; set; }
        public virtual TheCar TheCar { get; set; }
    }

    public enum CheckResultType { No, Yes }

}