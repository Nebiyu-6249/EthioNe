using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
namespace EthioNegarit.Core.Interfaces.Services
{

    public  interface ILogAuditService
    {
        Audit Add(Audit Audit);
        PagedList<Audit> GetAll(int pageIndex, int pageSize);
        PagedList<Audit> SearchAudit(string search, int pageIndex, int pageSize);
    }
}
