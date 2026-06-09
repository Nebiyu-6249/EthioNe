using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IVoteService
    {
      Vote Add(Vote vote);
        Vote Get(Guid id);
        void Delete(Vote vote);
        IList<Vote> GetAllVotesByUser(Guid membershipId);
        List<Vote> GetVotesByPosts(List<Guid> postIds);
        List<Vote> GetVotesByMessages(List<Guid> messageIds);
        List<Vote> GetVotesByPost(Guid postId);

      //  List<Like> GetLikesByReplies(List<Guid> replayIds);

    }
}
