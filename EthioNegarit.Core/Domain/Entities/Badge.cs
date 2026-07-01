using System;
using System.Collections.Generic;
using EthioNegarit.Utilities;
using EthioNegarit.Core.Domain.Enums;

namespace EthioNegarit.Core.Domain.Entities
{
  
    public partial class Badge : Entity
    {
        public Badge()
        {
            Id = GuidComb.GenerateComb();
        }

        /// <summary>
        /// Specifies the target badge interface names matched to the corresponding badge type
        /// </summary>
        public static readonly Dictionary<BadgeType, string> BadgeClassNames = new Dictionary<BadgeType, string>
                                                            {
                                                                {BadgeType.VoteUp, "EthioNegarit.Core.Interfaces.Badges.IVoteUpBadge"},
                                                                {BadgeType.MarkAsSolution, "EthioNegarit.Core.Interfaces.Badges.IMarkAsSolutionBadge"},
                                                                {BadgeType.Time, "EthioNegarit.Core.Interfaces.Badges.ITimeBadge"},
                                                                {BadgeType.Post, "EthioNegarit.Core.Interfaces.Badges.IPostBadge"},
                                                                {BadgeType.VoteDown, "EthioNegarit.Core.Interfaces.Badges.IVoteDownBadge"},
                                                                {BadgeType.Profile, "EthioNegarit.Core.Interfaces.Badges.IProfileBadge"},
                                                                {BadgeType.Favourite, "EthioNegarit.Core.Interfaces.Badges.IFavouriteBadge"},
                                                                {BadgeType.Tag, "EthioNegarit.Core.Interfaces.Badges.ITagBadge"},
                                                                {BadgeType.Point, "EthioNegarit.Core.Interfaces.Badges.IPointBadge"},
                                                                {BadgeType.Solution, "EthioNegarit.Core.Interfaces.Badges.ISolutionBadge"}
                                                            };

        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Milestone { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? AwardsPoints { get; set; }
        public virtual IList<MembershipUser> Users { get; set; }
    }
}
