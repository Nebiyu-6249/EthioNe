using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthioNegarit.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
  public  class FAQMapping : EntityTypeConfiguration<FAQ>
    {
        public FAQMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Question).IsRequired();
            Property(x => x.Replay).IsOptional();
           // Property(x => x.HelpCategory).IsOptional();
            Property(x => x.TimeStamp).IsRequired();
        }
    }
}
