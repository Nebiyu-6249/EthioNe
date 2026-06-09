using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IHelpService
    {
        FAQ Add(FAQ faq);
        void Delete(FAQ faq); 
        IList<FAQ> GetFAQs();
        FAQ GetFAQ(Guid id);
        PagedList<FAQ> Search(string search, string language, int pageIndex, int pageSize);
        PagedList<FAQ> GetFAQs(string language, int pageIndex, int pageSize);
        IList<FAQ> GetFAQ(string language);
    }
}
