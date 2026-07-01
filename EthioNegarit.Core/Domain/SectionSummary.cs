

namespace EthioNegarit.Core.Domain
{
    using Entities;
    using System.Collections.Generic;
    class SectionSummary
    {
        public Section Section { get; set; }
        public List<CategorySummary> CategorySummaries { get; set; }
    }
}
