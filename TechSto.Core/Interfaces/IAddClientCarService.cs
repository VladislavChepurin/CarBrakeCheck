using TechSto.Core.DTOs;

namespace TechSto.Core.Interfaces
{
    public interface IAddClientCarService
    {
        void SaveNewClientWithCars(SaveNewClientDto dto);
        void UpdateClientCar(UpdateClientCarDto dto);
    }
}
