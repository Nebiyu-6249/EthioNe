using EthioNegarit.Core.Domain.Entities;


namespace EthioNegarit.Core.Events
{
  public  class UpdateProfileEventArgs : EthioNegaritEventArgs
    {
        public MembershipUser User { get; set; }
    }
}
