using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class ChatRoomMemberMapping : EntityTypeConfiguration<ChatRoomMember>
    {

        public ChatRoomMemberMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();

            Property(x => x.LastActive).IsOptional();
            Property(x => x.EnableNotification).IsOptional();

            Property(x => x.LogOn).IsOptional();

            HasRequired(x => x.ChatRoom)
                .WithMany(x => x.ChatRoomMembers)
                .Map(x => x.MapKey("ChatRoom_Id"))
                .WillCascadeOnDelete(false);

            HasRequired(x => x.User)
               .WithMany(x => x.ChatRoomMembers)
               .Map(x => x.MapKey("MembershipUser_Id"))
               .WillCascadeOnDelete(false);

        }
    }
}
