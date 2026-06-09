using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
  public  class AddressAttribute : Attribute
    {     
        public int Address { get; set; }
        public AddressAttribute(int address)
        {
            Address = address;
        }
    }
}

    