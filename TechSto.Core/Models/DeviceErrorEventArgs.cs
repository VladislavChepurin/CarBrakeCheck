namespace TechSto.Core.Models
{
    public class DeviceErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }

        public Exception Exception { get; set; }

        public ErrorSeverity Severity { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
