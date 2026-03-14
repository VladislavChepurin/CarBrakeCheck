namespace TechSto.Core.Models 
{ 
    public class DeviceConnectionEventArgs : EventArgs
    { 
        public bool IsConnected { get; set; } 
        public string PortName { get; set; } 
        public DateTime Timestamp { get; set; }
    } 
}