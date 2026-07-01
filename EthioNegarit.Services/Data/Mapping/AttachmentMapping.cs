using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
    public class AttachmentMapping : EntityTypeConfiguration<Attachment>
    {
        public AttachmentMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Filename).IsRequired().HasMaxLength(200);
           Property(x => x.ContentType).IsOptional();
           // Property(x => x.Data).IsOptional();
           // Property(x => x.FileType).IsOptional();

            Property(x => x.DateCreated).IsRequired();

            HasRequired(x => x.PrivateMessage).WithMany(x => x.Attachments).Map(x => x.MapKey("PrivateMessage_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.MembershipUser).WithMany(x => x.Attachments).Map(x => x.MapKey("MembershipUser_Id")).WillCascadeOnDelete(false);
        }
    }
}
