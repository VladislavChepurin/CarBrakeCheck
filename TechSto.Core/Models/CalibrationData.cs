namespace TechSto.Core.Models
{
    public class CalibrationData
    {
        public byte[] RawData { get; set; } = new byte[32];
        public bool IsValid { get; set; }
    }
}
