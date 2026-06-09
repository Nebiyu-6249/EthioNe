using EthioNegarit.Core.Domain;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface ICategoryService
    {
        
         List<Category> GetAll();
        IEnumerable<Category> GetAllMainCategories();
        IEnumerable<Category> SearchCategories(string name);

        /// <summary>
        /// Gets categories that the user has access to (i.e. There access is not denied)
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <returns></returns>
        List<Category> GetAllowedCategories(MembershipRole role);

        /// <summary>
        /// Get category permissions for a specific permission
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <param name="actionType">Pass in the permission you want to check, for example 'Delete Posts' - This will return a list of categories that the user has permission to delete posts</param>
        /// <returns></returns>
        List<Category> GetAllowedCategories(MembershipRole role, string actionType);
        IEnumerable<Category> GetAllSubCategories(Guid parentId);
        Category Get(Guid id);
        IList<Category> Get(IList<Guid> ids, bool fullGraph = false);
        CategoryWithSubCategories GetBySlugWithSubCategories(string slug);
        Category Get(string slug);
        List<Category> GetCategoryParents(Category category, List<Category> allowedCategories);
        void Delete(Category category);
        Category Add(Category category);
        void UpdateSlugFromName(Category category);
        Category SanitizeCategory(Category category);
        List<Category> GetSubCategories(Category category, List<Category> allCategories, int level = 2);
        List<SelectListItem> GetBaseSelectListCategories(List<Category> allowedCategories);
        Category GetBySlug(string slug);
        IList<Category> GetBySlugLike(string slug);
        IList<Category> GetAllDeepSubCategories(Category category);
        // IList<Category> GetCategories(List<Guid> ids);
      //  IList<Category> GetCategories(List<Guid> catIds);

        List<Category> GetCategories(List<Guid> catIds);

        List<Section> GetAllSections();
        Section GetSection(Guid id);
        void DeleteSection(Guid id);
        Section AddSection(Section section);
        IEnumerable<CategorySummary> GetAllMainCategoriesInSummary();
      
        ILookup<Guid, CategorySummary> GetAllMainCategoriesInSummaryGroupedBySection();
        //   IList<Category> GetCategories(List<Guid> catIds, List<Category> allowedCategories);

        List<Category> GetCats(List<Guid> catIds);

        IList<Category> AllCats();

        Category GetNewsCategory();
    }
}
