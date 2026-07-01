using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Interfaces;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Services.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Services
{
   public partial class PrivacyPolicyService: IPrivacyPolicyService
    {
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="cacheService"></param>
        public PrivacyPolicyService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }

        /// <summary>
        /// Get the site settings from cache, if not in cache gets from database and adds into the cache
        /// </summary>
        /// <returns></returns>
        public PrivacyPolicy GetPolicy(bool useCache = true)
        {
            /*
            var cacheKey = string.Concat(CacheKeys.Help.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Help.FirstOrDefault(x => x.Id == id));
            */
            if (useCache)
            {
                var cachedPolicy = _cacheService.Get<PrivacyPolicy>(CacheKeys.PrivacyPolicy.StartsWith);
                if (cachedPolicy == null)
                {
                    cachedPolicy = GetPolicyLocal(false);
                    _cacheService.Set(CacheKeys.PrivacyPolicy.StartsWith, cachedPolicy, CacheTimes.OneDay);
                }
                return cachedPolicy;
            }
            return GetPolicyLocal(true);
        }

        private PrivacyPolicy GetPolicyLocal(bool addTracking)
        {
            var privacyPolicy = _context.PrivacyPolicy;
            return addTracking ? privacyPolicy.FirstOrDefault() : privacyPolicy.AsNoTracking().FirstOrDefault();
        }

        public PrivacyPolicy Add(PrivacyPolicy privacyPolicy)
        {
            return _context.PrivacyPolicy.Add(privacyPolicy);
        }

        public PrivacyPolicy Get(Guid id)
        {
            return _context.PrivacyPolicy.FirstOrDefault(x => x.Id == id);
        }
    }
}
