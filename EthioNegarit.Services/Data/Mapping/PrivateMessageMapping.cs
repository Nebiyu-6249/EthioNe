using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class PrivateMessageMapping : EntityTypeConfiguration<PrivateMessage>
    {
        public PrivateMessageMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.DateSent).IsRequired();
            Property(x => x.Message).IsRequired();
            Property(x => x.IsRead).IsRequired();
            Property(x => x.IsSentMessage).IsRequired();


         //   Property(x => x.IsStarred).IsRequired().HasDefaultValue(false);
            Property(x => x.IsStarred).IsRequired();

         /*   HasMany(x => x.Attachments).WithRequired(x => x.PrivateMessage)
            .Map(x => x.MapKey("PrivateMessage_Id"))
            .WillCascadeOnDelete(false);*/

        }
    }
}
