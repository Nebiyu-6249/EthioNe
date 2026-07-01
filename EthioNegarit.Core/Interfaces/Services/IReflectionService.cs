using System;
using System.Collections.Generic;
using System.Reflection;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IReflectionService
    {
        List<Assembly> GetAssemblies();
        bool InterfaceFilter(Type typeObj, Object criteriaObj);
    }
}
