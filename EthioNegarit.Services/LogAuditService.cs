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
    using Core.Domain.General;
    using Utilities;

    public partial class LogAuditService : ILogAuditService
    {
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        public LogAuditService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }
        public Audit Add(Audit audit)
        {
         
            return _context.Audit.Add(audit);
        }

        public PagedList<Audit> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = _context.Audit.Count();

            // Get the topics using an efficient
            var results = _context.Audit
                                .OrderByDescending(x => x.TimeAccessed)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();


            // Return a paged list
            return new PagedList<Audit>(results, pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// Gets tags by search term, in a paged list and grouped if any are duplicate
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedList<Audit> SearchAudit(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var totalCount = _context.Audit.Count(x => x.UserName.Contains(search));

            // Get the topics using an efficient
            var results = _context.Audit
                                .Where(x => x.UserName.Contains(search))
                                .OrderBy(x => x.TimeAccessed)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();


            // Return a paged list
            return new PagedList<Audit>(results, pageIndex, pageSize, totalCount);
        }

    }
}
