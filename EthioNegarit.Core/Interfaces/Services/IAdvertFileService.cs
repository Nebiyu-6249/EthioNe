using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
    
    public partial interface IAdvertFileService
    {
        AdvertisementFile Add(AdvertisementFile advertFile);
        void Delete(AdvertisementFile advertFile);
        IList<AdvertisementFile> GetAll();
        IList<AdvertisementFile> GetAllByAdvert(Guid advertId);
        IList<AdvertisementFile> GetAllByUser(Guid membershipUserId);
        AdvertisementFile Get(Guid id);
    }
}
