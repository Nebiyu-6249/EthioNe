using System;
using System.Collections.Generic;
namespace EthioNegarit.WebUI.ViewModels
{
    public class VoteBadgeViewModel
    {
        public Guid PostId { get; set; }
    }

    public class MarkAsSolutionBadgeViewModel
    {
        public Guid PostId { get; set; }
    }
    public class FavouriteViewModel
    {
        public Guid FavouriteId { get; set; }
    }

    public class PostBadgeViewModel
    {
        public Guid PostId { get; set; }
    }

    public class TimeBadgeViewModel
    {
        public Guid Id { get; set; }
    }   

    public class PointBadgeViewModel
    {
        public Guid Id { get; set; }
    }
    public class AllBadgesViewModel
    {
        public IList<Core.Domain.Entities.Badge> AllBadges { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public string Search { get; set; }

    }
    public class MemberBadgesViewModel
    {
       public IList<Core.Domain.Entities.Badge> MemberBadges { get; set; }    
    }

}
