using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Utilities;
using System.Collections.Generic;
namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class LatestUsersViewModels
    {
        public IList<MembershipUser> Users { get; set; }
    }
    public class LowestPointUsersViewModels
    {
        public Dictionary<MembershipUser, int> Users { get; set; }
    }
    public class LowestPointPostsViewModels
    {
        public IList<Post> Posts { get; set; }
    }
    public class HighestViewedTopics
    {
        public IList<Topic> Topics { get; set; }
    }
    public class RecentTopicsViewModel
    {
        public IList<RssItem> RssFeed { get; set; }
    }
    public class TodaysTopics
    {
        public IList<Topic> Topics { get; set; }
    }
}