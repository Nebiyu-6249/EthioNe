using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IAdService
    {
        Advertisement Add(Advertisement advert);
        Advertisement SanitizeAdvert(Advertisement advertisement);
        int GetPendingAdvertsCount();
        Advertisement GetAdvert(string slug);    
        Advertisement Get(Guid advertId);
        void Delete(Advertisement advert);
        PagedList<Advertisement> GetMemberAds(int pageIndex, int pageSize, MembershipUser user);
        PagedList<Advertisement> GetAllAds(int pageIndex, int pageSize, int amountToTake, AdViewQuery query);
        PagedList<Advertisement> GetActiveAds(int pageIndex, int pageSize, int amountToTake);    
        AdvertisementFile GetVideo(Guid advertId);    
        IList<AdvertisementFile> GetVideos(Guid advertId);
        IList<Advertisement> GetActiveAds();
        IList<Advertisement> GetActiveAds(string country);
      //  IList<Advertisement> GetMemberAds(MembershipUser user);
    }
}
