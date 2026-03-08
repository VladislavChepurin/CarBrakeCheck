namespace TechSto.Core.DTOs
{
    public class SaveNewClientDto
    {
        public bool IsNewOwner { get; set; }
        public string OwnerName { get; set; } = "";
        public string OwnerSurname { get; set; } = "";
        public int? ExistingOwnerId { get; set; }
        public string ExistingOwnerName { get; set; } = "";
        public string ExistingOwnerSurname { get; set; } = "";
        public List<ClientCarItemDto> Cars { get; set; } = new();
    }
}
