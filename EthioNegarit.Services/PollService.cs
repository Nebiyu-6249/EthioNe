

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
    public partial class PollService : IPollService
    {
        
         private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;

        public PollService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }

        public List<Poll> GetAllPolls()
        {
            var cacheKey = string.Concat(CacheKeys.Poll.StartsWith, "GetAllPolls");
            return _cacheService.CachePerRequest(cacheKey, () => _context.Poll.ToList());
        }

        public Poll Add(Poll poll)
        {
            poll.DateCreated = DateTime.UtcNow;
            poll.IsClosed = false;
            return _context.Poll.Add(poll);
        }

        public Poll Get(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.Poll.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Poll.FirstOrDefault(x => x.Id == id));
        }

        public void Delete(Poll item)
        {
            _context.Poll.Remove(item);
        }
        
    }
}
