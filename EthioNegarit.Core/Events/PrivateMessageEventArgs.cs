using EthioNegarit.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Events
{
  public  class PrivateMessageEventArgs : EthioNegaritEventArgs
    {
        public PrivateMessage PrivateMessage { get; set; }
    }
}
