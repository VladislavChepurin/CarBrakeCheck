using TechSto.Core.DTOs;
using TechSto.Core.Entities;
using TechSto.Core.Interfaces;
using TechSto.Infrastructure.Data;

namespace TechSto.Infrastructure.Services
{
    public class AddClientCarService: IAddClientCarService
    {
        
        private readonly MainContext _context;

        public AddClientCarService(MainContext context)
        {
            _context = context;
        }

        public void SaveNewClientWithCars(SaveNewClientDto dto)
        {
            using var tx = _context.Database.BeginTransaction();

            Owner? owner;
            if (dto.IsNewOwner)
            {
                owner = new Owner { Name = dto.OwnerName, Surname = dto.OwnerSurname };
                _context.Owners.Add(owner);
            }
            else
            {
                owner = _context.Owners.Find(dto.ExistingOwnerId!.Value);
                if (owner != null)
                {
                    owner.Name = dto.ExistingOwnerName;
                    owner.Surname = dto.ExistingOwnerSurname;
                }
            }

            foreach (var item in dto.Cars)
            {
                var car = new TheCar
                {
                    GosNumber = item.GosNumber,
                    VinCode = item.VinCode,
                    CarModelId = item.CarModelId
                };

                if (dto.IsNewOwner)
                    car.Owner = owner!;
                else
                    car.OwnerId = owner!.Id;

                _context.TheCars.Add(car);
            }

            _context.SaveChanges();
            tx.Commit();
        }

        public void UpdateClientCar(UpdateClientCarDto dto)
        {
            var car = _context.TheCars.Find(dto.CarId)
                ?? throw new InvalidOperationException(/*Properties.Resources.ErrorCarNotFound*/);

            using var tx = _context.Database.BeginTransaction();

            car.GosNumber = dto.GosNumber;
            car.VinCode = dto.VinCode;
            car.CarModelId = dto.CarModelId;

            if (dto.OwnerId.HasValue)
            {
                var owner = _context.Owners.Find(dto.OwnerId.Value);
                if (owner != null)
                {
                    owner.Name = dto.OwnerName;
                    owner.Surname = dto.OwnerSurname;
                    car.OwnerId = owner.Id;
                }
            }
            _context.SaveChanges();
            tx.Commit();
        }
    }
}
