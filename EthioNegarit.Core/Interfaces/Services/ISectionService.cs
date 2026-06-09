using EthioNegarit.Core.Domain.Entities;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface ISectionService
    {
        IList<Section> GetSections();
     //   void Delete(Section section);
        Section Add(Section section);
        List<Section> GetAllSections();
        Section GetSection(int sortOrder);
        Section GetNewsSection();
    }
}
