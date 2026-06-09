namespace EthioNegarit.WebUI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.Enums;
    using Application;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Domain.General;
    using Application.ActionFilterAttributes;

    public class ActiveMembersViewModel
    {
        public IList<MembershipUser> ActiveMembers { get; set; }
    }

    public class MemberAddViewModel
    {
        [Required]
        [ForumResourceDisplayName("Members.Label.Username")]
        [StringLength(150, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        /*required pattern="[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}"*/
        [ForumResourceDisplayName("Members.Label.EmailAddress")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        [ForumResourceDisplayName("Members.Label.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        [ForumResourceDisplayName("Members.Label.UserIsApproved")]
        public bool IsApproved { get; set; }

        [ForumResourceDisplayName("Members.Label.Comment")]
        public string Comment { get; set; }

        [ForumResourceDisplayName("Members.Label.Roles")]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string SpamAnswer { get; set; }
        public string ReturnUrl { get; set; }
        public string SocialProfileImageUrl { get; set; }
        public string UserAccessToken { get; set; }
        public LoginType LoginType { get; set; }

        public bool EmailNotification { get; set; }
        public bool DisablePosting { get; set; }
        public bool DisablePrivateMessages { get; set; }
        public bool DisableFileUploads { get; set; }
        public bool DisableChat { get; set; }
        public bool DisableAd { get; set; }
        public int SpamCount { get; set; }

       // [ForumResourceDisplayName("TermsAndConditions.Label.Agree")]
      //  [MustBeTrue(ErrorMessage = "TermsAndConditions.Label.AgreeError")]
        public bool Agree { get; set; }
    }

    public class MemberFrontEndEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [ForumResourceDisplayName("Members.Label.Username")]
        [StringLength(150, MinimumLength = 4)]
        public string UserName { get; set; }

        [ForumResourceDisplayName("Members.Label.EmailAddress")]
        [EmailAddress]
        [Required]
      //  required pattern = "[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}"
        public string Email { get; set; }

        [ForumResourceDisplayName("Members.Label.Signature")]
        [StringLength(1000)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Signature { get; set; }

        [ForumResourceDisplayName("Members.Label.Age")]
        [Range(0, int.MaxValue)]
        public int? Age { get; set; }

        [ForumResourceDisplayName("Members.Label.Location")]
        [StringLength(100)]
        public string Location { get; set; }

        [ForumResourceDisplayName("Members.Label.Website")]
        [Url]
        [StringLength(100)]
        public string Website { get; set; }

        [ForumResourceDisplayName("Members.Label.Twitter")]
        [Url]
        [StringLength(60)]
        public string Twitter { get; set; }

        [ForumResourceDisplayName("Members.Label.UploadNewAvatar")]
        public HttpPostedFileBase[] Files { get; set; }

        [ForumResourceDisplayName("Members.Label.Facebook")]
        [Url]
        [StringLength(60)]
        public string Facebook { get; set; }

        public string Avatar { get; set; }
        public bool DisableFileUploads { get; set; }

        [ForumResourceDisplayName("Members.Label.DisableEmailNotifications")]
        public bool DisableEmailNotifications { get; set; }


        public int AmountOfPoints { get; set; }

    }

    public class LogOnViewModel
    {
        public string ReturnUrl { get; set; }
        [Required]
        [ForumResourceDisplayName("Members.Label.Username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }
        [ForumResourceDisplayName("Members.Label.RememberMe")]
        public bool RememberMe { get; set; }
    }

    public class ViewMemberViewModel
    {
        public MembershipUser User { get; set; }
        public Guid LoggedOnUserId { get; set; }
        public PermissionSet Permissions { get; set; }
        public bool IsBlocked { get; set; }
      //  public bool YouBlocked { get; set; }
        public bool IsBlockedYou { get; set; }
    }

   /* public class ViewMemberDiscussionsViewModel
    {
        public IList<TopicViewModel> Topics { get; set; }
    }*/

  /*  public class ViewAdminSidePanelViewModel
    {
        public MembershipUser CurrentUser { get; set; }
        public int NewPrivateMessageCount { get; set; }
        public bool CanViewPrivateMessages { get; set; }
        public bool IsDropDown { get; set; }
        public int ModerateCount { get; set; }
    }*/
  /*  public class AdminMemberProfileToolsViewModel
    {
        public MembershipUser CurrentUser { get; set; }
    }*/

    public class AutocompleteViewModel
    {
        public string label { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

  /*  public class ReportMemberViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Reason { get; set; }
    }*/

    public class ListLatestMembersViewModel
    {
        public Dictionary<string, string> Users { get; set; }
    }

    public class PublicMemberListViewModel
    {
        public IList<PublicSingleMemberListViewModel> Users { get; set; }
        public IList<MembershipUser> Members { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public string Search { get; set; }
        public int? TotalPages { get; set; }
    }

    public class PublicSingleMemberListViewModel
    {
        [ForumResourceDisplayName("Members.Label.Username")]
        public string UserName { get; set; }

        public string NiceUrl { get; set; }

        [ForumResourceDisplayName("Members.Label.DateJoined")]
        public DateTime CreateDate { get; set; }

        public int TotalPoints { get; set; }

        public string Avatar { get; set; }

        public Guid Id { get; set; }

        public string Email { get; set; }

        public int Badge { get; set; }
	//	public int TopicCount{ get; set; }
		//public int PostCount{ get; set; }
		public bool MemberIsOnline { get; set; }   
        public DateTime LastLoginDate { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string web { get; set; }
    }

    public class ForgotPasswordViewModel
    {
      //  [EmailAddress]
        [Required]
        [ForumResourceDisplayName("Members.Label.EmailAddressBlank")]    
        public string EmailAddress { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.CurrentPassword")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.ConfirmNewPassword")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.ConfirmNewPassword")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }
   
    public class CloseAccountViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [ForumResourceDisplayName("Members.Label.CurrentPassword")]
        public string OldPassword { get; set; }
        [ForumResourceDisplayName("Members.Account.CloseDays")]
        public int Days { get; set; }

        /* [Required]
         [StringLength(100, MinimumLength = 6)]
         [DataType(DataType.Password)]
         [ForumResourceDisplayName("Members.Label.NewPassword")]
         public string NewPassword { get; set; }

         [DataType(DataType.Password)]
         [ForumResourceDisplayName("Members.Label.ConfirmNewPassword")]
         [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
         public string ConfirmPassword { get; set; }*/
    }
    public class RecentMembersViewModel
    {
        public IList<MembershipUser> RecentMembers { get; set; }
		}
	/*public class MembershipViewModel
		{
		public MembershipUser User{ get; set; }
		public int PostsCount{ get; set; }
		public int TopicsCount{ get; set; }
		}*/

	public class MemberPopUpViewModel
		{
		public MembershipUser User{ get; set; }
		public int PostsCount{ get; set; }
		public int TopicsCount{ get; set; }

		}
    public class DashboardViewModel
    {
        public MembershipUser User { get; set; }
        public int ReportCount { get; set; }
        public int SpamCount { get; set; }
        public string AccountStatus { get; set; }
    }
    public class NavPanelViewModel
    {
        public MembershipUser CurrentUser { get; set; }
        public int ReportCount { get; set; }
        public int SpamCount { get; set; }
        public string AccountStatus { get; set; }
    }
    public class AdminPanelViewModel
    {
      //  public MembershipUser CurrentUser { get; set; }
        public int PrivateMessageCount { get; set; }
      //  public bool CanViewPrivateMessages { get; set; }
      //  public bool IsDropDown { get; set; }
        public int ModerateCount { get; set; }
        public int ReportCount { get; set; }
        public int AdvertCount { get; set; }
        public int PaymentCount { get; set; }

    }
}