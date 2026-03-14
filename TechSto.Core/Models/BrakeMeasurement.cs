namespace TechSto.Core.Models
{
    public class BrakeMeasurement
    {
        public int RightWeight1 { get; set; }
        public int RightWeight2 { get; set; }
        public int LeftWeight1 { get; set; }
        public int LeftWeight2 { get; set; }
        public int PedalPressure { get; set; }
        public int RightBrakeSensor { get; set; }
        public int LeftBrakeSensor { get; set; }
        public DiscreteSignals DiscreteSignals { get; set; }
        public bool IsValid { get; set; }
        public DateTime Timestamp { get; set; }

        // Вычисляемые свойства
        public int TotalRightWeight => RightWeight1 + RightWeight2;
        public int TotalLeftWeight => LeftWeight1 + LeftWeight2;
        public int TotalWeight => TotalRightWeight + TotalLeftWeight;
        public float BrakeForceDifference => Math.Abs(RightBrakeSensor - LeftBrakeSensor);
    }
}