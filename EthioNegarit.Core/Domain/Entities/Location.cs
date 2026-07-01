using System;
using EthioNegarit.Utilities;
namespace EthioNegarit.Core.Domain.Entities
{
    
    public partial class Location : Entity
    {
        public Location()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CityName { get; set; }
        public string Description { get; set; }
        public virtual Advertisement Advertisement { get; set; }
    }
}
