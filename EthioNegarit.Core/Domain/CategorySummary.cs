using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain
{
  public  class CategorySummary
    {
        public Category Category { get; set; }
        public int TopicCount { get; set; }
        public int PostCount { get; set; }
        public Topic MostRecentTopic { get; set; }
    }
}
