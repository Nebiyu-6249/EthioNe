using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IBadgeService
    {         
        /// <summary>
        /// Bring the database into line with the badge classes found at runtime
        /// </summary>
        /// <returns>Set of valid badge classes to use when assigning badges</returns>
        void SyncBadges(List<Assembly> assemblies);
        bool ProcessBadge(BadgeType badgeType, MembershipUser user);
        PagedList<Badge> GetPagedGroupedBadges(int pageIndex, int pageSize);        
        PagedList<Badge> SearchPagedGroupedBadges(string search, int pageIndex, int pageSize);
        IList<Badge> GetallBadges();
        void Delete(Badge badge);

        /// <summary>
        /// Deletes the last checked time
        /// </summary>
        /// <param name="badgeTypeTimeLastChecked"></param>
        void DeleteTimeLastChecked(BadgeTypeTimeLastChecked badgeTypeTimeLastChecked);

        /// <summary>
        /// Get a badge by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Badge GetBadge(string name);

        Badge Get(Guid id);

        /// <summary>
        /// All badges
        /// </summary>
        /// <returns></returns>
        IEnumerable<Badge> GetAll();
      
        Badge Add(Badge newBadge);
        
    }
}
