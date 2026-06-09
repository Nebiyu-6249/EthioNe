
using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;


namespace EthioNegarit.Core.EqualityComparers
{
    public class PermissionEqualityComparer : IEqualityComparer<Permission>
        
    {
        public bool Equals(Permission x, Permission y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(Permission obj)
        {
            return obj.Id.GetHashCode();
        }
        
    }
}
