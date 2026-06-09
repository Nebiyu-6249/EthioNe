using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
{
    public class BreadcrumbViewModel
    {
        public List<Category> Categories { get; set; }
        public Topic Topic { get; set; }
        public Category Category { get; set; }
    }

    public class CategoryListViewModel
    {
        public Dictionary<Category, PermissionSet> AllPermissionSets { get; set; }
    }


  

    public class SubCategoryViewModel
    {
        public Dictionary<Category, PermissionSet> AllPermissionSets { get; set; }
        public Category ParentCategory { get; set; }
    }
    public class ListCategoriesViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }

    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            AllCategories = new List<SelectListItem>();
            AllSections = new List<SelectListItem>();
        }
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Category Name")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Category Colour")]
        [UIHint(AppConstants.EditorTemplateColourPicker), AllowHtml]
        public string CategoryColour { get; set; }

        [DisplayName("Lock The Category")]
        public bool IsLocked { get; set; }

        [DisplayName("Moderate all topics in this Category")]
        public bool ModerateTopics { get; set; }

        [DisplayName("Moderate all posts in this Category")]
        public bool ModeratePosts { get; set; }

        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        [DisplayName("Parent Category")]
        public Guid? ParentCategory { get; set; }
        [DisplayName("Section")]
        public Guid Section { get; set; }
        public List<SelectListItem> AllCategories { get; set; }
        public List<SelectListItem> AllSections { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Category Image")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }
            public List<TopicViewModel> Topics { get; set; }
            public PermissionSet Permissions { get; set; }
            public Category Category { get; set; }
            public CategoryListViewModel SubCategories { get; set; }
            public MembershipUser User { get; set; }
            public bool IsSubscribed { get; set; }

            public int? PageIndex { get; set; }
            public int? TotalCount { get; set; }
            public int? TotalPages { get; set; }
            public int PostCount { get; set; }

            // Topic info
            public Topic LatestTopic { get; set; }
            public int TopicCount { get; set; }

            // Misc
            public bool ShowUnSubscribedLink { get; set; }
    }

    public class DeleteCategoryViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public Category Category { get; set; }
        public List<Category> SubCategories { get; set; }
    }


    public class CategorySectionViewModel
    {
        public Section Section { get; set; }
        public Dictionary<CategorySummary, PermissionSet> AllPermissionSets { get; set; }
    }

   /* public class CategoryRssViewModel
    {
        public IList<RssItem> RssFeed { get; set; }
    }*/
   /* public class CategoryRssLinkViewModel
    {
        public RssItem RssLink { get; set; }
    }*/
   /* public class CategoryRsViewModel
    {
        public List<RssItem> RssFeed { get; set; }
    }*/
}