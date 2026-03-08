namespace TechSto.Core.DTOs
{
    public class ClientRecordDto
    {
        public int CarId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerSurname { get; set; }
        public string? GosNumber { get; set; }
        public string? VinCode { get; set; }
        public string? BrandName { get; set; }
        public string? Model { get; set; }
        public string? LastTestDateString { get; set; } 
        public DateTime? LastTestDate { get; set; }      // преобразованная дата для привязки
        public string? CategoryName { get; set; }
        public int AxlesCount { get; set; }
        public int MaxMass { get; set; }  //Максимльная масса
        public int CurbMass { get; set; } //Снаряженная масса     

        public string Owner {
            get => $"{OwnerSurname} {OwnerName}";
        }        
    }
}
