using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class ChatMessageMapping : EntityTypeConfiguration<ChatMessage>
    {
        public ChatMessageMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Timestamp).IsRequired();
            Property(x => x.Content).IsRequired().HasMaxLength(450);       
            Property(x => x.FlaggedAsSpam).IsOptional();
            Property(x => x.Deleted).IsOptional();           
            Property(x => x.IsReplay).IsOptional();
        //    Property(x => x.DisableReplay).IsOptional();
            Property(x => x.InReplyTo).IsOptional();
            HasRequired(c => c.ChatRoom).WithMany(c => c.ChatMessage).Map(m => m.MapKey("ChatRoom_Id")).WillCascadeOnDelete(false);
            HasRequired(t => t.User).WithMany(t => t.ChatMessage).Map(m => m.MapKey("MembershipUser_Id")).WillCascadeOnDelete(false);
        }
    }
}
