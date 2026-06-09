using EthioNegarit.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
namespace EthioNegarit.Services.Data.Mapping
{    
    public class AdvertisementMapping : EntityTypeConfiguration<Advertisement>
    {
        public AdvertisementMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();         
            Property(x => x.Name).IsRequired().HasMaxLength(200);
            Property(x => x.MotoSlogan).IsOptional().HasMaxLength(200);
            Property(x => x.About).IsOptional().HasMaxLength(600);
            Property(x => x.Line).IsOptional();
            Property(x => x.Email).IsOptional();
            Property(x => x.Address).IsOptional();
            Property(x => x.Country).IsOptional();
            Property(x => x.DateStart).IsRequired();
            Property(x => x.DateEnd).IsRequired();
            Property(x => x.LinkUrl).IsOptional().HasMaxLength(200);
            Property(x => x.LinkText).IsOptional().HasMaxLength(200);
            Property(x => x.ReminderLastSent).IsOptional();
            Property(x => x.Slug).IsRequired().HasMaxLength(450)
                                .HasColumnAnnotation("Index",
                                new IndexAnnotation(new IndexAttribute("IX_Advertisement_Slug", 1) { IsUnique = true }));      
            Property(x => x.DateCreated).IsRequired();     
            Property(x => x.PostCount).IsOptional();         
            Property(x => x.DateLastActive).IsRequired();
            Property(x => x.DateUpdate).IsRequired();
            Property(x => x.AdStatus).IsRequired();
            Property(x => x.EnableReminder).IsOptional();
            Property(x => x.Image).IsOptional();
            Property(x => x.Month).IsOptional();
            HasOptional(t => t.Package).WithOptionalDependent().Map(m => m.MapKey("Package_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.User)
              .WithMany(x => x.Advertisements)
              .Map(x => x.MapKey("MembershipUser_Id"))
              .WillCascadeOnDelete(false);
            // Ignores
            Ignore(x => x.NiceUrl);
        }
        }
}
