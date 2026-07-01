using EthioNegarit.Core.Domain.Entities;


namespace EthioNegarit.Core.Events
{
  public  class MarkedAsSolutionEventArgs : EthioNegaritEventArgs
    {
        public Topic Topic { get; set; }
        public Post Post { get; set; }
        public MembershipUser Marker { get; set; }
        public MembershipUser SolutionWriter { get; set; }

     //   cust
     //   public ChatMessages ChatMessages { get; set; }
     //   public ChatReplayMessage ChatReplayMessage { get; set; }

  
    }
}
