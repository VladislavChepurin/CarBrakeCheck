using TechSto.Core.Entities;

namespace TechSto.WPF.ViewModels
{
    public class AxleRowViewModel : ViewModelBase
    {
        private RotationDirection _rotationDirection = RotationDirection.Forward;
        private bool _hasParkingBrake;
        private BrakeType _brakeType = BrakeType.Disc;
        private bool _hasRegulator;

        public int Order { get; set; }

        public RotationDirection RotationDirection
        {
            get => _rotationDirection;
            set { _rotationDirection = value; OnPropertyChanged(); }
        }

        public bool HasParkingBrake
        {
            get => _hasParkingBrake;
            set { _hasParkingBrake = value; OnPropertyChanged(); }
        }

        public BrakeType BrakeType
        {
            get => _brakeType;
            set { _brakeType = value; OnPropertyChanged(); }
        }

        public bool HasRegulator
        {
            get => _hasRegulator;
            set { _hasRegulator = value; OnPropertyChanged(); }
        }

        public static AxleRowViewModel Default(int order) => new()
        {
            Order = order,
            RotationDirection = RotationDirection.Forward,
            HasParkingBrake = false,
            BrakeType = BrakeType.Disc,
            HasRegulator = false
        };

        public Axle ToAxle() => new()
        {
            Order = Order,
            RotationDirection = RotationDirection,
            HasParkingBrake = HasParkingBrake,
            BrakeType = BrakeType,
            HasRegulator = HasRegulator
        };
    }
}
