using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;


namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPermissionService
    {
      IEnumerable<Permission> GetAll();
        Permission Add(Permission permission);
        void Delete(Permission permission);
        Permission Get(Guid id);
       
    }
}
