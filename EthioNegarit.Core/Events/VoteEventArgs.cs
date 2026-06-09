using EthioNegarit.Core.Domain.Entities;


namespace EthioNegarit.Core.Events
{
  public  class VoteEventArgs : EthioNegaritEventArgs
    {
        public Vote Vote { get; set; }
    }
}
