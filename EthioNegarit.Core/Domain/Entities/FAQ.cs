using EthioNegarit.Utilities;
using System;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class FAQ : Entity
    {
        public FAQ()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Question { get; set; }
     //   public HelpCategory HelpCategory { get; set; }
        public string Replay { get; set; }
        public string Language { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
