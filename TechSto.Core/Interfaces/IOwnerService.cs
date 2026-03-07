using System.Collections.ObjectModel;
using TechSto.Core.Entities;

namespace TechSto.Core.Interfaces
{
    public interface IOwnerService
    {
        IQueryable<Owner> GetAll(bool includeCars = true, bool includeChecks = true);
        Owner GetById(int id);
        ObservableCollection<Owner> GetLocalOwners();
        void Add(Owner owner);
        Owner CreateOwner(string name);
        void Update(Owner owner);
        void UpdateFull(Owner updatedOwner);
        void Delete(int id);
        void AddDataCheck(int ownerId, Check check);
        void DeleteDataCheck(int checkId);
    }
}
