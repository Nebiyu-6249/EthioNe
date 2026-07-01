using EthioNegarit.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Events
{
  public  interface IEventHandler
    {
       void RegisterHandlers(EventManager theEventManager);
    }
}
