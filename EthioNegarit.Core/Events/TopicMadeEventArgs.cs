using EthioNegarit.Core.Domain.Entities;


namespace EthioNegarit.Core.Events
{
   public class TopicMadeEventArgs : EthioNegaritEventArgs
    {
        public Topic Topic { get; set; }
    }
}
