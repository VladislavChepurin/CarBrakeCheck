using TechSto.Core.Models;

namespace TechSto.Core.DTOs
{
    public class ClientRecordMessageDto
    {
        public bool Start { get; set; } //Признак старта
        public string?  CarName { get; set; }
        public string? GosNumber { get; set; }      
        public int AxlesCount { get; set; }
        public string? CarCategory { get; set; }
        public bool IsRelativeDifference { get; set; }
        public bool SelectedMeasurementMode { get; set; }
        public MeasurementType SelectedMeasurementType { get; set; }
    }
}
