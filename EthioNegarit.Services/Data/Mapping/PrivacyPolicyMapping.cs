using EthioNegarit.Core.Domain.Entities;
using System.Data.Entity.ModelConfiguration;
namespace EthioNegarit.Services.Data.Mapping
{
    public  class PrivacyPolicyMapping : EntityTypeConfiguration<PrivacyPolicy>
    {
        public PrivacyPolicyMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Terms).IsOptional();
            Property(x => x.Version).IsOptional();
            Property(x => x.LastUpdate).IsOptional();
            Property(x => x.EffectiveDate).IsOptional();            
            Property(x => x.TermsAm).IsOptional();
            Property(x => x.Policy).IsOptional();
            Property(x => x.PolicyAm).IsOptional();
        }
    }
}
