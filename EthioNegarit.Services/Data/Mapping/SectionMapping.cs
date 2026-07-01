using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;


namespace EthioNegarit.Services.Data.Mapping
{
 public class SectionMapping: EntityTypeConfiguration<Section>
    {       
          public SectionMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(450);
            Property(x => x.Description).IsOptional();
            Property(x => x.DateCreated).IsRequired();
            Property(x => x.SortOrder).IsRequired();

            HasMany(x => x.Categories)
              .WithRequired(x => x.Section)
              .Map(x => x.MapKey("Section_Id"))
              .WillCascadeOnDelete(false);
        }
    
    }
}
