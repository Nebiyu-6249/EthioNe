using EthioNegarit.Utilities;
using System;
namespace EthioNegarit.Core.Domain.Entities
{
    public partial class PrivacyPolicy : Entity
    {
    public PrivacyPolicy()
    {
        Id = GuidComb.GenerateComb();
    }
        public Guid Id { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Version { get; set; }
        public string Terms { get; set; }
        public string TermsAm { get; set; }
        public string Policy { get; set; }
        public string PolicyAm { get; set; }
    }
}
