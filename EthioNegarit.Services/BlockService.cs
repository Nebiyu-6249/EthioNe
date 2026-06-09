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

    public partial class BlockService : IBlockService
    {        
         private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        public BlockService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }
        public Block Add(Block block)
        {
            block.Date = DateTime.UtcNow;
            return _context.Block.Add(block);
        }
        public void Delete(Block block)
        {
            _context.Block.Remove(block);
        }
        public Block Get(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.Block.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Block.FirstOrDefault(x => x.Id == id));
        }
       /* public IList<Block> GetBlockedMembers(int amountToTake, Guid userId)
        {
                var total = _context.Block.AsNoTracking().Count(x => x.Blocker.Id == userId);
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                var results = _context.Block
                                      .Include(x => x.Blocked)
                                      .AsNoTracking()                                   
                                         .Where(x => x.Blocker.Id == userId);

                var blockedMembers = results.Take(amountToTake).ToList();
                return new List<Block>(blockedMembers).ToList();
        }*/
        public PagedList<Block> GetBlockedMembers(int pageIndex, int pageSize, Guid userId)
        {
             //   var totalCount = _context.MembershipUser.Count();
            var total = _context.Block.AsNoTracking().Count(x => x.Blocker.Id == userId);
            var results = _context.Block
                         .Include(x => x.Blocked)
                         .AsNoTracking()
                         .Where(x => x.Blocker.Id == userId)
                         .OrderBy(x => x.Date)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)                      
                          .ToList();

                return new PagedList<Block>(results, pageIndex, pageSize, total);
           
        }
        public IList<Block> GetMembersBlocked(Guid blockedId, Guid blockerId)
        {
          //  var cacheKey = string.Concat(CacheKeys.Block.StartsWith, "GetMembersBlockedYou-", blockedId, "-", blockerId);
            return (
                _context.Block
                .Where(x => x.Blocked.Id == blockedId && x.Blocker.Id == blockerId).ToList());
        }
    }
}
