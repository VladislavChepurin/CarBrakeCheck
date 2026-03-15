using System.IO.Ports;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;

namespace TechSto.Infrastructure.Services
{
    public class SerialPortDiscoveryService : ISerialPortDiscoveryService
    {
        public IReadOnlyList<SerialPortInfo> GetAvailablePorts()
        {
            return SerialPort.GetPortNames()
                .OrderBy(x => x)
                .Select(x => new SerialPortInfo
                {
                    PortName = x,
                    DisplayName = x,
                    IsAvailable = true
                })
                .ToArray();
        }
    }
}
