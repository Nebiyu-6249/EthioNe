using EthioNegarit.Core.Domain.Activity;
using System.ComponentModel.DataAnnotations;
using EthioNegarit.Core.Domain.Entities;
using System.Collections.Generic;
using EthioNegarit.Core.Domain.General;
namespace EthioNegarit.WebUI.ViewModels
{
    public class ListCategoriesViewModels
    {
        public MembershipUser MembershipUser { get; set; }
        public MembershipRole MembershipRole { get; set; }
    }
    public class AllRecentActivitiesViewModel
    {
        public PagedList<ActivityBase> Activities { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
         public int? TotalPages { get; set; }
    }
    public class FAQViewModel
    {
        public IEnumerable<FAQ> FAQ { get; set; }
        public string Search { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }
    public class RssFeedViewModel
    {
      //  public IList<RssItem> RssFeed { get; set; }
        public IList<Utilities.RssItem> RssFeed { get; set; }
    }


    public class ContactUsViewModel
    {
        /*
           // Validates a property that should not start with certain special characters
    [RegularExpression(@"^(.*\\S.*)$", ErrorMessage = "Name cannot be only whitespace.")]
    //   [RegularExpression(@"^[a-zA-Z0-9]{2,30}(?:[a-zA-Z0-9.,'_ -]*[a-zA-Z0-9]{2,30})?$", ErrorMessage = "Name cannot be only whitespace.")]
    [RegularExpression(@"^(.*\\S.*)$", ErrorMessage = "Enter valid Name")]
        */
        [Required]

        //  [ForumResourceDisplayName("Members.Label.Username")]
        //  [StringLength(150, MinimumLength = 2)]
        //  [RegularExpression(@"^[(.*\\S.*)]?$", ErrorMessage = "Enter valid Name Name cannot be only whitespace.")]
        //^[A-Z][a-z]+$
        [RegularExpression(@"^[a-zA-Z0-9]{2,30}(?:[a-zA-Z0-9.,'_ -]*[a-zA-Z0-9]{2,30})+$", ErrorMessage = "Enter valid Name")]
       // [RegularExpression(@"^[(.*\\S.*)]$", ErrorMessage = "Enter valid Name")]
       // [RegularExpression(@"^(.*\\S.*)$", ErrorMessage = "Enter valid Name")]
        public string Name { get; set; }

       // [ForumResourceDisplayName("Members.Label.Comment")]
        public string Subject { get; set; }

        [Required]
        //  [ForumResourceDisplayName("ContactUs.Label.Message")]
        [StringLength(6000, MinimumLength = 4)]
        public string Message { get; set; }
        /* [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]*/
        /*        [RegularExpression(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}$", ErrorMessage = "Invalid email format.")]*/
        [Required]
       [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]

        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class CopyUrlViewModel
    {
        // public RssItem RssLink { get; set; }
        public Core.Domain.General.RssItem RssLink { get; set; }
    }
    }