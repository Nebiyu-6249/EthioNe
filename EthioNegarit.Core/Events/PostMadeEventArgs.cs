using EthioNegarit.Core.Domain.Entities;


namespace EthioNegarit.Core.Events
{
 public class PostMadeEventArgs : EthioNegaritEventArgs
    {
        public Post Post { get; set; }
    }
}
