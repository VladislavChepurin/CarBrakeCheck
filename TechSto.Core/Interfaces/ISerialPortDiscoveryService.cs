using TechSto.Core.Models;

namespace TechSto.Core.Interfaces
{
    public interface ISerialPortDiscoveryService
    {
        IReadOnlyList<SerialPortInfo> GetAvailablePorts();
    }
}
