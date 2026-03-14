using TechSto.Core.Interfaces;
using TechSto.Core.Messaging;

namespace TechSto.WPF.ViewModels
{
    public class MeasurementsViewModel : ViewModelBase, IDisposable
    {
        private readonly IBrakeTesterService _device;
        private readonly IMessageBus _bus;
  
        public MeasurementsViewModel(IBrakeTesterService brakeTester, IMessageBus bus)
        {
            _device = brakeTester;
            _bus = bus; 
        }

        public async Task Connect(string port) 
        { 
            await _device.ConnectAsync(port);
        }

        public async Task Disconnect() 
        { 
            await _device.DisconnectAsync();
        }

        public async Task LoadCalibration() 
        {
            var calibration = await _device.RequestCalibrationDataAsync(); 
        }

        public void Dispose()
        {
            
        }
    }
}