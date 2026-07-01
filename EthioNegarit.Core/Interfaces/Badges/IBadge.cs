using EthioNegarit.Core.Domain.Entities;

namespace EthioNegarit.Core.Interfaces.Badges
{
    public interface IBadge
    {
        bool Rule(MembershipUser user);
    }
}
