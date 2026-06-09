using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class FavouriteMapping : EntityTypeConfiguration<Favourite>
    {
        public FavouriteMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.DateCreated).IsRequired();
            HasRequired(x => x.Post).WithMany(x => x.Favourites).Map(x => x.MapKey("Post_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.Member).WithMany(x => x.Favourites).Map(x => x.MapKey("MembershipUser_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.Topic).WithMany(x => x.Favourites).Map(x => x.MapKey("Topic_Id")).WillCascadeOnDelete(false);
        }
    }
}
