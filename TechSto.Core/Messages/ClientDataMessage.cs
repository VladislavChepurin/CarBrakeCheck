using TechSto.Core.Models;

namespace TechSto.Core.Messages
{
    public class ClientDataMessage
    {
        public bool Start { get; set; } //Признак старта
        public string?  CarName { get; set; }
        public string? GosNumber { get; set; }      
        public int AxlesCount { get; set; }
        public string? CarCategory { get; set; }
        public bool IsRelativeDifference { get; set; }
        public bool SelectedMeasurementMode { get; set; }
        public int CurrentAxles { get; set; }
        public MeasurementType SelectedMeasurementType { get; set; }
        public string? ComPortName { get; set; }


    }
}
