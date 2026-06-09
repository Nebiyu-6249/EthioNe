namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using Core.Domain.Enums;
    public partial class AdService : IAdService
    {
        private readonly EthioNegaritContext _context;
        private readonly ISettingsService _settingsService;
        private readonly IBadgeService _badgeService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;
        private readonly IConfigService _configService;
        private readonly IEmailService _emailService;
        public AdService(ISettingsService settingsService, IConfigService configService, IBadgeService badgeService, ILoggingService loggingService, IEthioNegaritContext context, ICacheService cacheService, IEmailService emailService)
        {
            _badgeService = badgeService;
            _loggingService = loggingService;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _configService = configService;
            _emailService = emailService;
            _context = context as EthioNegaritContext;
        }
        public Advertisement SanitizeAdvert(Advertisement advertisement)
        {
            advertisement.Name = StringUtils.GetSafeHtml(advertisement.Name);
            advertisement.About = StringUtils.GetSafeHtml(advertisement.About);
            // Check settings
            if (_settingsService.GetSettings().EnableEmoticons == true)
            {
                advertisement.About = _configService.Emotify(advertisement.About);
            }
            return advertisement;
        }
        public Advertisement Get(Guid advertId)
        {
            var cacheKey = string.Concat(CacheKeys.Advertisement.StartsWith, "Get-", advertId);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var advertisement = _context.Advertisement
                                    .Include(x => x.User)
                                .FirstOrDefault(x => x.Id == advertId);
                return advertisement;
            });
        }
        public Advertisement GetAdvert(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);
            return _context.Advertisement
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.Slug == slug);
        }
        public IList<Advertisement> GetBySlugLike(string slug)
        {
            return _context.Advertisement
                            .Where(x => x.Slug.Contains(slug))
                            .ToList();
        }
        public Advertisement Add(Advertisement advertisement)
        {
            advertisement = SanitizeAdvert(advertisement);
            advertisement.DateCreated = DateTime.UtcNow;
            // url slug generator
            advertisement.Slug = ServiceHelpers.GenerateSlug(advertisement.Name, GetBySlugLike(ServiceHelpers.CreateUrl(advertisement.Name)), null);
            return _context.Advertisement.Add(advertisement);
        }
        public int GetPendingAdvertsCount()
        {
            var cacheKey = string.Concat(CacheKeys.Advertisement.StartsWith, "GetPendingAdvertsCount-");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Advertisement.AsNoTracking().Count(x => x.AdStatus == AdStatus.Partial);
            });
        }      
        public void Delete(Advertisement advert)
        {
            _context.Advertisement.Remove(advert);
        }
        public PagedList<Advertisement> GetMemberAds(int pageIndex, int pageSize,MembershipUser user)
        {
            var total = _context.Advertisement.AsNoTracking().Where(x => x.User.Id == user.Id).Count();
           /* if (amountToTake < total)
            {
                total = amountToTake;
            }*/
            var results = _context.Advertisement
                                .Where(x => x.User.Id == user.Id)                           
                                .OrderByDescending(x => x.DateCreated)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            // Return a paged list
            return new PagedList<Advertisement>(results, pageIndex, pageSize, total);
        }            
        public AdvertisementFile GetVideo(Guid advertId)
        {
                var adVideoFile = _context.AdvertisementFile
                                   .Where(x => x.FileType == "mp4")
                                .FirstOrDefault(x => x.Advertisement.Id == advertId);
                return adVideoFile;
        }
        public IList<AdvertisementFile> GetVideos(Guid advertId)
        {
            return _context.AdvertisementFile
                 .Where(x => x.FileType == "mp4" && x.Advertisement.Id == advertId)
                .ToList();
        }      
        public PagedList<Advertisement> GetAllAds(int pageIndex, int pageSize, int amountToTake, AdViewQuery query)
        {
            var cacheKey = string.Concat(CacheKeys.Advertisement.StartsWith, "GetAllAds-", pageIndex, "-", pageSize, "-", amountToTake, "-", query);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var total = _context.Advertisement.AsNoTracking().Count();
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                // Get the ads using an efficient
                var results = _context.Advertisement
                                    .AsNoTracking()
                                    .Include(x => x.User)
                                     .Include(x => x.Payments)
                                      .Include(x => x.Package);

                // Filter the paging out
                switch (query)
                {
                    case AdViewQuery.All:
                        results = results.OrderByDescending(x => x.DateCreated);
                        break;
                    case AdViewQuery.Active:
                        results = results.Where(x => x.AdStatus == AdStatus.Active);
                        //  results = results.OrderByDescending(x => x.AdStatus == AdStatus.Active).ThenByDescending(x => x.DateCreated);
                        break;
                    case AdViewQuery.Pending:
                        results = results.Where(x => x.AdStatus == AdStatus.Pending);
                        break;
                    case AdViewQuery.Completed:
                        results = results.Where(x => x.AdStatus == AdStatus.Completed);
                        break;
                    case AdViewQuery.Partial:
                        results = results.Where(x => x.AdStatus == AdStatus.Partial);
                        break;
                    default:
                        results = results.OrderByDescending(x => x.DateCreated);
                        break;
                }
                results = results.OrderByDescending(x => x.DateCreated);
                var advertisements = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                // Return a paged list
                return new PagedList<Advertisement>(advertisements, pageIndex, pageSize, total);

            });
        }
        public IList<Advertisement> GetActiveAds()
        {
            var timeSpan = DateTime.UtcNow.AddMinutes(AppConstants.TimeSpanInMinutesToShowAd);
            var total = _context.Advertisement.AsNoTracking().Count(x => x.AdStatus == AdStatus.Active);
            if (total > 5) {
               timeSpan = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowAd);
            }         
            var results = _context.Advertisement
                .Where(x => x.AdStatus == AdStatus.Active && (x.DateLastActive < timeSpan || x.DateLastActive == null))
                               .OrderBy(x => x.DateLastActive)
                                .ToList();
            return results;
        }
        public IList<Advertisement> GetActiveAds(string country)
        {
            var results = GetActiveAds();
            return results.Where(x => x.Country == country)
                .OrderBy(x => x.DateLastActive)
                .ToList();
        }       
        public PagedList<Advertisement> GetActiveAds(int pageIndex, int pageSize, int amountToTake)
        {
            // We might only want to display the top 10
            // but there might not be 10 ads
            var total = _context.Advertisement.AsNoTracking().Count(x => x.AdStatus == AdStatus.Active);
            if (amountToTake < total)
            {
                total = amountToTake;
            }
            var results = _context.Advertisement
                                .AsNoTracking()
                                .Include(x => x.User)
                                .Where(x => x.AdStatus == AdStatus.Active)
                                .OrderBy(x => x.DateLastActive)
                               // .OrderByDescending(x => x.DateLastActive)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            return new PagedList<Advertisement>(results, pageIndex, pageSize, total);
        }       
    }
}
