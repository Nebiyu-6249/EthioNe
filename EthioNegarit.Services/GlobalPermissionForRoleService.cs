

namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Domain.Entities;
    using System.Data.Entity;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    
    public partial class GlobalPermissionForRoleService : IGlobalPermissionForRoleService
    {
       
         private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;

        public GlobalPermissionForRoleService(IEthioNegaritContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }

        public GlobalPermissionForRole Add(GlobalPermissionForRole permissionForRole)
        {
            return _context.GlobalPermissionForRole.Add(permissionForRole);
        }

        public void Delete(GlobalPermissionForRole permissionForRole)
        {
            _context.GlobalPermissionForRole.Remove(permissionForRole);
        }

        public GlobalPermissionForRole CheckExists(GlobalPermissionForRole permissionForRole)
        {
            if (permissionForRole.Permission != null && permissionForRole.MembershipRole != null)
            {
                return Get(permissionForRole.Permission.Id, permissionForRole.MembershipRole.Id);
            }

            return null;
        }

        public Dictionary<Permission, GlobalPermissionForRole> GetAll(MembershipRole role)
        {
            var cacheKey = string.Concat(CacheKeys.GlobalPermissionForRole.StartsWith, "GetAll-", role.Id);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var catRowList = _context.GlobalPermissionForRole.Include(x => x.MembershipRole).Where(x => x.MembershipRole.Id == role.Id).ToList();
                // Group by permission first - duplicate rows for the same role/permission
                // would otherwise throw "An item with the same key has already been added"
                return catRowList
                    .Where(catRow => catRow.Permission != null)
                    .GroupBy(catRow => catRow.Permission.Id)
                    .ToDictionary(grp => grp.First().Permission, grp => grp.First());
            });
        }

        public Dictionary<Permission, GlobalPermissionForRole> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.GlobalPermissionForRole.StartsWith, "GetAll");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var catRowList = _context.GlobalPermissionForRole.Include(x => x.MembershipRole).ToList();
                // Across all roles the same Permission entity can appear more than once,
                // so group first to guarantee unique dictionary keys
                return catRowList
                    .Where(catRow => catRow.Permission != null)
                    .GroupBy(catRow => catRow.Permission.Id)
                    .ToDictionary(grp => grp.First().Permission, grp => grp.First());
            });
        }

        public GlobalPermissionForRole Get(Guid permId, Guid roleId)
        {
            var cacheKey = string.Concat(CacheKeys.GlobalPermissionForRole.StartsWith, "Get-", permId, "-", roleId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.GlobalPermissionForRole
                                                                            .Include(x => x.MembershipRole)
                                                                            .FirstOrDefault(x => x.Permission.Id == permId && x.MembershipRole.Id == roleId));
        }

        public GlobalPermissionForRole Get(Guid permId)
        {
            var cacheKey = string.Concat(CacheKeys.GlobalPermissionForRole.StartsWith, "Get-", permId);
            return _cacheService.CachePerRequest(cacheKey, () => _context.GlobalPermissionForRole
                                                                             .Include(x => x.MembershipRole)
                                                                             .FirstOrDefault(x => x.Id == permId));
        }

        public void UpdateOrCreateNew(GlobalPermissionForRole globalPermissionForRole)
        {
            // Firstly see if this exists already
            var permission = CheckExists(globalPermissionForRole);

            // if it exists then just update it
            if (permission != null)
            {
                permission.IsTicked = globalPermissionForRole.IsTicked;
            }
            else
            {
                Add(globalPermissionForRole);
            }
        }
        
    }
}
