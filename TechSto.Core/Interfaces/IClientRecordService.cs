using TechSto.Core.DTOs;

namespace TechSto.Core.Interfaces
{
    public interface IClientRecordService
    {
        List<ClientRecordDto> LoadClientRecords();
        void DeleteClientRecord(int carId);
    }
}
