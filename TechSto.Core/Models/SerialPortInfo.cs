namespace TechSto.Core.Models
{
    public class SerialPortInfo
    {
        public string PortName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

        public override string ToString() => DisplayName;
    }
}