using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
  public  class SortAttribute : Attribute
    {
        public int SortOrder { get; set; }

        public SortAttribute(int sortOrder)
        {
            SortOrder = sortOrder;
        }
    }
}
