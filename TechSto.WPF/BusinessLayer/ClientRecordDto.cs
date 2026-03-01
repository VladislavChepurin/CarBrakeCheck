namespace TechSto.WPF.BusinessLayer
{
    public class ClientRecordDto
    {
        public int CarId { get; set; }
        public string? Owner { get; set; }
        public string? StateNumber { get; set; }
        public string? Vin { get; set; }
        public string? BrandName { get; set; }
        public string? Model { get; set; }
        public string? LastTestDateString { get; set; } 
        public DateTime? LastTestDate { get; set; }      // преобразованная дата для привязки
    }
}
