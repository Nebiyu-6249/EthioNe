using System;
namespace EthioNegarit.Core.Events
{
    public abstract class EthioNegaritEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
