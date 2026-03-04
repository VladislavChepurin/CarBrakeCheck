namespace TechSto.ViewModels
{
    public class CarRowViewModel
    {
        public int CarId { get; set; }
        public int OwnerId { get; set; }
        public string Owner { get; set; } = "";
        public string StateNumber { get; set; } = "";
        public string Vin { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string Model { get; set; } = "";
        public DateTime? LastTestDate { get; set; }
    }
}
