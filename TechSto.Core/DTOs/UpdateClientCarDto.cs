namespace TechSto.Core.DTOs
{
    public class UpdateClientCarDto
    {
        public int CarId { get; set; }
        public string GosNumber { get; set; } = "";
        public string VinCode { get; set; } = "";
        public int CarModelId { get; set; }
        public int? OwnerId { get; set; }
        public string OwnerName { get; set; } = "";
        public string OwnerSurname { get; set; } = "";
    }
}
