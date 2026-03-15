namespace TechSto.Core.Messages
{
    public sealed class DeviceAvailabilityChangedMessage
    {
        public bool IsAvailable { get; init; }
        public string? PortName { get; init; }
    }
}
