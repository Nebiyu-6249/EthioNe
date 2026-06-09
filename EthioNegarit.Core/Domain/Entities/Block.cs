using EthioNegarit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Entities
{
    public class Block : Entity
    {

        public Block()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public MembershipUser Blocker { get; set; }
        public MembershipUser Blocked { get; set; }
        public DateTime Date { get; set; }
    }
}
