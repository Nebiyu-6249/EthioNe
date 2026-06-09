using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IReflectionService
    {
        List<Assembly> GetAssemblies();
        bool InterfaceFilter(Type typeObj, Object criteriaObj);
    }
}
