namespace TechSto.Core.Models
{
    public class MeasurementReceivedEventArgs : EventArgs
    {
        public BrakeMeasurement Measurement { get; set; }

        public byte[] RawData { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
