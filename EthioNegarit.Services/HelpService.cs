namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Core.Domain.Enums;
    using Core.Domain.General;
    using Utilities;

    public partial class HelpService : IHelpService
    {
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        public HelpService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }

        public FAQ Add(FAQ fAQ)
        {
            fAQ.TimeStamp = DateTime.UtcNow;
            return _context.FAQ.Add(fAQ);
        }
       public FAQ GetFAQ(Guid id)
        {
         //   var cacheKey = string.Concat(CacheKeys.FAQ.StartsWith, "Get-", id);
           // return _cacheService.CachePerRequest(cacheKey, () => 
           return _context.FAQ.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(FAQ faq)
        {
            _context.FAQ.Remove(faq);
        }
        public IList<FAQ> GetFAQs()
        {
            var helps = _context.FAQ
                //.Where(x => x.HelpCategory == HelpCategory.Advertisement)
                .OrderByDescending(x => x.Language)
                .ToList();
            return helps;
        }
       /* public IList<FAQ> GetFAQ(HelpCategory helpCategory)
        {
            var helps = _context.FAQ
               // .Where(x => x.HelpCategory == helpCategory)
                .OrderByDescending(x => x.TimeStamp)
                .ToList();
            return helps;
        }*/
        public PagedList<FAQ> Search(string search, string language, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.FAQ
                      .Where(x => x.Question.ToUpper().Contains(search.ToUpper()) || x.Question.ToUpper().Contains(search.ToUpper()) || x.Replay.ToUpper().Contains(search.ToUpper())
                      //.Where(x.Language == language)
                      );
            var results = query
                .OrderBy(x => x.Question)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
            return new PagedList<FAQ>(results, pageIndex, pageSize, query.Count());
        }
        public PagedList<FAQ> GetFAQs(string language, int pageIndex, int pageSize)
        {
            var query = _context.FAQ
            // .Where(x => x.Question.ToUpper().Contains(search.ToUpper()) || x.Question.ToUpper().Contains(search.ToUpper()) || x.Replay.ToUpper().Contains(search.ToUpper()));
            .Where(x => x.Language == language);
            var results = query
                .OrderBy(x => x.Question)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
            return new PagedList<FAQ>(results, pageIndex, pageSize, query.Count());
        }
        public IList<FAQ> GetFAQ(string language)
        {
            var helps = _context.FAQ
                .Where(x => x.Language == language)
                .OrderByDescending(x => x.TimeStamp)
                .ToList();
            return helps;
        }
    }
}
