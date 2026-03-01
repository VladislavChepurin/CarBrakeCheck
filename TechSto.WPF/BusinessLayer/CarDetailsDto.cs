using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechSto.WPF.BusinessLayer
{
    public class CarDetailsDto
    {
        public int CarId { get; set; }
        public string Owner { get; set; }
        public string StateNumber { get; set; }
        public string Vin { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public int AxlesCount { get; set; }
        public decimal MaxMass { get; set; }
        public float BrakeForceDifference { get; set; }

    }
}
