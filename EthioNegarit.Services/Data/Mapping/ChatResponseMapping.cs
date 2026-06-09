using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
  public class ChatResponseMapping : EntityTypeConfiguration<ChatResponse>
    {
        public ChatResponseMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();

            Property(x => x.DateResponse).IsRequired();

            HasRequired(x => x.ChatMessage).WithMany(x => x.ChatResponses).Map(x => x.MapKey("ChatMessage_Id")).WillCascadeOnDelete(false);

            HasRequired(x => x.User).WithMany(x => x.ChatResponses).Map(x => x.MapKey("MembershipUser_Id")).WillCascadeOnDelete(false);

         //   HasOptional(t => t.MembershipUserRespond).WithOptionalDependent(x => x.ChatRespond).Map(m => m.MapKey("MembershipUserRespond_Id"));

         //   HasOptional(t => t.MembershipUserRespond).WithOptionalDependent().Map(m => m.MapKey("MembershipUserRespond_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.MembershipUserRespond).WithMany(x => x.ChatRespondes).Map(x => x.MapKey("MembershipUserRespond_Id")).WillCascadeOnDelete(false);

        }
    }
}
