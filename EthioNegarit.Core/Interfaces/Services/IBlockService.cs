using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IBlockService
    {
       Block Add(Block block);
        void Delete(Block block);
        Block Get(Guid id);
      //  IList<Block> GetBlockedMembers(int amountToTake, Guid userId);      
       // IList<Block> GetMembersBlockedYou(Guid blockedId,Guid blockerId);
        IList<Block> GetMembersBlocked(Guid blockedId, Guid blockerId);
        PagedList<Block> GetBlockedMembers(int pageIndex, int pageSize, Guid userId);

    }
}
