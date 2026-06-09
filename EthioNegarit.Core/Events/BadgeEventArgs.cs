using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Events
{
    public class BadgeEventArgs : EthioNegaritEventArgs
    {
        public MembershipUser User { get; set; }
        public BadgeType BadgeType { get; set; }
    }
}
