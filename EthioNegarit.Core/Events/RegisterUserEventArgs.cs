using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;

namespace EthioNegarit.Core.Events
{
   public class RegisterUserEventArgs : EthioNegaritEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipCreateStatus CreateStatus { get; set; }
    }
}
