

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
    using Core.Events;
    using Core.Domain.Enums;

    public partial class VoteService : IVoteService
    {
        
         private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;

        public VoteService(IEthioNegaritContext context, IMembershipUserPointsService membershipUserPointsService, ICacheService cacheService)
        {
            _membershipUserPointsService = membershipUserPointsService;
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }

        public Vote Get(Guid id)
        {
            return _context.Vote.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Vote vote)
        {
            // Delete any points associated with this vote
            _membershipUserPointsService.Delete(PointsFor.Vote, vote.Id);

            // Delete the vote
            _context.Vote.Remove(vote);
        }

        public IList<Vote> GetAllVotesByUser(Guid membershipId)
        {
            return _context.Vote.Where(x => x.VotedByMembershipUser.Id == membershipId).ToList();
        }

        public List<Vote> GetVotesByPosts(List<Guid> postIds)
        {
            var cacheKey = string.Concat(CacheKeys.BannedEmail.StartsWith, "GetVotesByPosts-", postIds.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Vote
                            .Include(x => x.VotedByMembershipUser)
                            .Include(x => x.User)
                            .Include(x => x.Post)
                            .AsNoTracking()
                            .Where(x => postIds.Contains(x.Post.Id)).ToList();
            });
        }

        public List<Vote> GetVotesByPost(Guid postId)
        {
            return _context.Vote
            .Include(x => x.VotedByMembershipUser)
            .Include(x => x.User)
            .Include(x => x.Post)
            .Where(x => x.Post.Id == postId).ToList();
        }

        /// <summary>
        /// Add a new vote
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public Vote Add(Vote vote)
        {

            var e = new VoteEventArgs {Vote = vote};
            EventManager.Instance.FireBeforeVoteMade(this, e);

            if (!e.Cancel)
            {
                _context.Vote.Add(vote);

                EventManager.Instance.FireAfterVoteMade(this, new VoteEventArgs {Vote = vote});
            }

            return vote;
        }

        public List<Vote> GetVotesByMessages(List<Guid> messageIds)
        {
            var cacheKey = string.Concat(CacheKeys.BannedEmail.StartsWith, "GetVotesByMessages-", messageIds.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Vote
                            .Include(x => x.VotedByMembershipUser)
                            .Include(x => x.User)
                            .Include(x => x.Post)
                            .AsNoTracking()
                            .Where(x => messageIds.Contains(x.Post.Id)).ToList();
            });
        }


      /*  public List<Like> GetLikesByReplies(List<Guid> replayIds)
        {
            var cacheKey = string.Concat(CacheKeys.BannedEmail.StartsWith, "GetLikesByReplay-", replayIds.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Like
                            .Include(x => x.LikedByMembershipUser)
                            .Include(x => x.User)
                            .Include(x => x.ChatReplayMessage)
                            .AsNoTracking()
                            .Where(x => replayIds.Contains(x.ChatReplayMessage.Id)).ToList();
            });
        }*/
       /* public Like Add(Like like)
        {

            var e = new VoteEventArgs { Vote = vote };
            EventManager.Instance.FireBeforeVoteMade(this, e);

            if (!e.Cancel)
            {
                _context.Vote.Add(vote);

                EventManager.Instance.FireAfterVoteMade(this, new VoteEventArgs { Vote = vote });
            }

            return vote;
        }*/
    }
}
/*
https://adminlte.io/docs/3.1/

*/
