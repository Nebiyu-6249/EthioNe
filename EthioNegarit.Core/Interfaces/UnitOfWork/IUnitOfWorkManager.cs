using System;

namespace EthioNegarit.Core.Interfaces.UnitOfWork
{
    public partial interface IUnitOfWorkManager : IDisposable
    {       
        IUnitOfWork NewUnitOfWork();
    }
}
