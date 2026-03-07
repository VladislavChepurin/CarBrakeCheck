using TechSto.DataBase.Entity;

namespace TechSto.BusinessLayer
{
    public class SaveNewClientDto
    {
        public bool IsNewOwner { get; set; }
        public string OwnerName { get; set; } = "";
        public string STSNumber { get; set; } = "";
        public int? ExistingOwnerId { get; set; }
        public string ExistingOwnerName { get; set; } = "";
        public string ExistingOwnerSTS { get; set; } = "";
        public List<ClientCarItemDto> Cars { get; set; } = new();
    }

    public class ClientCarItemDto
    {
        public string GosNumber { get; set; } = "";
        public string BodyNumber { get; set; } = "";
        public int CarModelId { get; set; }
    }

    public class UpdateClientCarDto
    {
        public int CarId { get; set; }
        public string GosNumber { get; set; } = "";
        public string BodyNumber { get; set; } = "";
        public int CarModelId { get; set; }
        public int? OwnerId { get; set; }
        public string OwnerName { get; set; } = "";
        public string STSNumber { get; set; } = "";
    }

    public class AddClientCarService
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
                owner = new Owner { Name = dto.OwnerName, STSNumber = dto.STSNumber };
                _context.Owners.Add(owner);
            }
            else
            {
                owner = _context.Owners.Find(dto.ExistingOwnerId!.Value);
                if (owner != null)
                {
                    owner.Name = dto.ExistingOwnerName;
                    owner.STSNumber = dto.ExistingOwnerSTS;
                }
            }

            foreach (var item in dto.Cars)
            {
                var car = new TheCar
                {
                    GosNumber = item.GosNumber,
                    BodyNumber = item.BodyNumber,
                    FrameNumber = "",
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
                ?? throw new InvalidOperationException(Properties.Resources.ErrorCarNotFound);

            using var tx = _context.Database.BeginTransaction();

            car.GosNumber = dto.GosNumber;
            car.BodyNumber = dto.BodyNumber;
            car.CarModelId = dto.CarModelId;

            if (dto.OwnerId.HasValue)
            {
                var owner = _context.Owners.Find(dto.OwnerId.Value);
                if (owner != null)
                {
                    owner.Name = dto.OwnerName;
                    owner.STSNumber = dto.STSNumber;
                    car.OwnerId = owner.Id;
                }
            }

            _context.SaveChanges();
            tx.Commit();
        }
    }
}
