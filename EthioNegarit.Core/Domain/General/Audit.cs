using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.General
{
   public class Audit
    {
        public Guid AuditID { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public string URLAccessed { get; set; }
        public string Platform { get; set; }
        public DateTime TimeAccessed { get; set; }

        public Audit()
        {
        }
    }
}


//Device
//Browser
//Country
//Region
//Header