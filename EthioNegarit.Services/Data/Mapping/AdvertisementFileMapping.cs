using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class AdvertisementFileMapping : EntityTypeConfiguration<AdvertisementFile>
    {
        public AdvertisementFileMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Filename).IsRequired().HasMaxLength(200);
            Property(x => x.ContentType).IsOptional();
            Property(x => x.Data).IsOptional();

            Property(x => x.FileType).IsOptional();

            Property(x => x.DateCreated).IsRequired();

            HasOptional(x => x.Advertisement).WithMany(x => x.Files).Map(x => x.MapKey("Advertisement_Id")).WillCascadeOnDelete(false);
           //To Be add Here
          //  HasOptional(x => x.MembershipUser).WithMany(x => x.AdvertisementFiles).Map(x => x.MapKey("MembershipUser_Id")).WillCascadeOnDelete(false);
        }
    }
}
