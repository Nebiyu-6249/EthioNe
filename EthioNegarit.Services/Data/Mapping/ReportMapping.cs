using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
  public  class ReportMapping : EntityTypeConfiguration<Report>
    {
        public ReportMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Reason).IsRequired().HasMaxLength(400);
            Property(x => x.DateReport).IsRequired();       
            HasRequired(x => x.ReportedPost).WithMany(x => x.Reports).Map(x => x.MapKey("Post_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.Reporter).WithMany(x => x.ReportOnUsers).Map(x => x.MapKey("Reporter_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.ReportedMember).WithMany(x => x.ReportedByUsers).Map(x => x.MapKey("ReportedMember_Id")).WillCascadeOnDelete(false);
        }
    }
}
