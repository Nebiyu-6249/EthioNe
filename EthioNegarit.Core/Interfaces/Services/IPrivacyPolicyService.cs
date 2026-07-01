using System;
using EthioNegarit.Core.Domain.Entities;
namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPrivacyPolicyService
    {
        PrivacyPolicy GetPolicy(bool useCache = true);
        PrivacyPolicy Add(PrivacyPolicy privacyPolicy);
        PrivacyPolicy Get(Guid id);
    }
}
