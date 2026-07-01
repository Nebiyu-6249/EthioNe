using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.General
{
    public class MarkAsSolutionReminder
    {
        public Topic Topic { get; set; }
        public int PostCount { get; set; }
    }
}
