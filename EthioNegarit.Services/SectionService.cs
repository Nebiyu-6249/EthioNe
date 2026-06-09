using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    using Utilities;
    using System.Web;
    using Core.Exceptions;
    using Core.Domain.General;
    using Core.Domain.Enums;
    public partial class SectionService : ISectionService
    {
        private readonly EthioNegaritContext _context;
        private readonly IBadgeService _badgeService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;

        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly ISettingsService _settingsService;
        public SectionService(IBadgeService badgeService, ILoggingService loggingService, IEthioNegaritContext context, ICacheService cacheService, IMembershipUserPointsService membershipUserPointsService, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _badgeService = badgeService;
            _loggingService = loggingService;
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;

            _membershipUserPointsService = membershipUserPointsService;
        }
        public IList<Section> GetSections()
        {
            var cacheKey = string.Concat(CacheKeys.Section.StartsWith, "GetSections");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Section
                      
                        .OrderBy(x => x.Name)
                        .ToList();
            });
        }
        public List<Section> GetAllSections()
        {
            return _context.Section.AsNoTracking()
                .Include(x => x.Categories)
                .OrderBy(x => x.SortOrder).ToList();
        }

        /// <inheritdoc />
        public Section GetSection(int sortOrder)
        {
            //  return _context.Section.Find(id);
            return _context.Section
                .FirstOrDefault(x => x.SortOrder == sortOrder);
              //  .Include(x => x.Categories);
        }

        public Section GetNewsSection()
        {
            //  return _context.Section.Find(id);
            return _context.Section
                .FirstOrDefault(x => x.SortOrder == AppConstants.NewsEventSortOrder);
            //  .Include(x => x.Categories);
        }

        public Section Add(Section section)
        {
            // Sanitize
            // Set the create date
            section.DateCreated = DateTime.UtcNow;

            // url slug generator
         
            // Add the category
            return _context.Section.Add(section);
        }
      /*  public void Delete(Section section)
        {
            // Check if anyone else if using this role
            var okToDelete = !category.Topics.Any();

            if (okToDelete)
            {
                // Get any categorypermissionforoles and delete these first
                var rolesToDelete = _categoryPermissionForRoleService.GetByCategory(category.Id);

                foreach (var categoryPermissionForRole in rolesToDelete)
                {
                    _categoryPermissionForRoleService.Delete(categoryPermissionForRole);
                }

                var categoryNotificationsToDelete = new List<CategoryNotification>();
                categoryNotificationsToDelete.AddRange(category.CategoryNotifications);
                foreach (var categoryNotification in categoryNotificationsToDelete)
                {
                    _categoryNotificationService.Delete(categoryNotification);
                }

                _context.Category.Remove(category);
            }
            else
            {
                var inUseBy = new List<Entity>();
                inUseBy.AddRange(category.Topics);
                throw new InUseUnableToDeleteException(inUseBy);
            }
        }
        */

    }
}
