using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    
  public  class BonusAttribute : Attribute
    {
        public int Bonus { get; set; }

        public BonusAttribute(int bonus)
        {
            Bonus = bonus;
        }
    }
}
