using EthioNegarit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class BannedWord
    {
       
          public BannedWord()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Word { get; set; }
        public bool? IsStopWord { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
