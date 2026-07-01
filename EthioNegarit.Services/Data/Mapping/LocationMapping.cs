using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
  public  class LocationMapping : EntityTypeConfiguration<Location>
    {
        public LocationMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Latitude).IsRequired();
            Property(x => x.Longitude).IsRequired();
            Property(x => x.CityName).IsOptional().HasMaxLength(200);
            Property(x => x.Description).IsOptional().HasMaxLength(200);

            HasRequired(x => x.Advertisement)
             .WithMany(x => x.Locations)
             .Map(x => x.MapKey("Advertisement_Id"))
             .WillCascadeOnDelete(false);
        }
    }
}
