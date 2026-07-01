using EthioNegarit.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Events
{
  public  interface IEventManager
    {
        /// <summary>
        /// Use reflection to get all event handling classes. Call this ONCE.
        /// </summary>
        void Initialize(ILoggingService loggingService, List<Assembly> assemblies);
    }
}
