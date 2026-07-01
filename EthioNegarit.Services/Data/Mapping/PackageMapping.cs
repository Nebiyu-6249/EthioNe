using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class PackageMapping : EntityTypeConfiguration<Package>
    {
        public PackageMapping()
        {
            HasKey(x => x.Id);

            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(50);
            Property(x => x.Description).IsOptional();
            Property(x => x.Type).IsOptional();         
            Property(x => x.DisplayName).IsOptional().HasMaxLength(100);
            Property(x => x.PriceRate).IsOptional();
            Property(x => x.Bonus).IsOptional();
            Property(x => x.Discount).IsRequired();
            Property(x => x.Files).IsOptional();
            Property(x => x.Address).IsOptional();
            Property(x => x.EnableVideo).IsOptional();
            Property(x => x.SortOrder).IsOptional();
            Property(x => x.Color).IsOptional();

            //   Property(x => x.AdDuration).IsRequired();
            //   Property(x => x.EnableFile).IsOptional();
            // Property(x => x.Duration).IsOptional();
            // Property(x => x.Month).IsOptional();
            // Property(x => x.TimeSpan).IsOptional();
            //Property(x => x.Image).IsOptional();
            //  Property(x => x.ImagesAllowed).IsOptional();
        }
    }
}
