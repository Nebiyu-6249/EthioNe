using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Services
    {
    public partial interface IPostService
        {

        Post SanitizePost (Post post);
        Post GetTopicStarterPost (Guid topicId);
        IEnumerable<Post> GetAll (List<Category> allowedCategories);
        IList<Post> GetLowestVotedPost (int amountToTake);
        IList<Post> GetHighestVotedPost (int amountToTake);
        IList<Post> GetByMember (Guid memberId, int amountToTake, List<Category> allowedCategories);
        IList<Post> GetReplyToPosts (Post post);
        IList<Post> GetReplyToPosts (Guid postId);
        IEnumerable<Post> GetPostsByFavouriteCount (Guid postsByMemberId, int minAmountOfFavourites);
        IEnumerable<Post> GetPostsFavouritedByOtherMembers (Guid postsByMemberId);
        PagedList<Post> SearchPosts (int pageIndex, int pageSize, int amountToTake, string searchTerm, List<Category> allowedCategories);
        PagedList<Post> GetPagedPostsByTopic (int pageIndex, int pageSize, int amountToTake, Guid topicId, PostOrderBy order);
        PagedList<Post> GetPagedPendingPosts (int pageIndex, int pageSize, List<Category> allowedCategories);
        IList<Post> GetPendingPosts (List<Category> allowedCategories, MembershipRole usersRole, Guid userId);
        IList<Post> GetAllPendingPosts();
       // int GetPendingPostsCount ();
        Post Add (Post post);
        Post Get (Guid postId);
        IList<Post> GetPostsByTopics (List<Guid> topicIds, List<Category> allowedCategories);
        bool Delete (Post post, IUnitOfWork unitOfWork, bool ignoreLastPost);
        IList<Post> GetSolutionsByMember (Guid memberId, List<Category> allowedCategories);
        int PostCount (List<Category> allowedCategories);
        Post AddNewPost (string postContent, Topic topic, MembershipUser user, out PermissionSet permissions);
        string SortSearchField (bool isTopicStarter, Topic topic, IList<TopicTag> tags);
      //  IList<Post> GetPostsByMember (Guid memberId, List<Category> allowedCategories);
        IList<Post> GetAllSolutionPosts (List<Category> allowedCategories);
       // IList<Post> GetPostsByTopic (Guid topicId);
        IEnumerable<Post> GetAllWithTopics (List<Category> allowedCategories);
        bool PassedPostFloodTest (MembershipUser user);

   //     IList<Post> GetPosts (List<Guid> postIds);

        List<Post> GetPostNotifications (List<Guid> topicIds, MembershipUser user);
        IList<Post> GetMemberSpamPost (MembershipUser user);
        IList<Post> GetReportedPosts ();
        PagedList<Post> GetMemberPosts(int pageIndex, int pageSize, int amountToTake, Guid memberGuid, List<Category> allowedCategories);

        }
    }
