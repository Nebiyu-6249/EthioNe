using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.WebUI.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
{
    public class CreateAdButtonViewModel
    {
        public bool UserCanCreatAd { get; set; }
    }
	public class CreateEditAdViewModel
    {
        [Required]
        [StringLength(200)]
        [ForumResourceDisplayName("Ad.Label.CompanyService")]
        public string Name { get; set; }
        [ForumResourceDisplayName("Ad.Label.MotoSlogan")]        
        public string Moto { get; set; }
        [UIHint(AppConstants.EditorType), AllowHtml]
        [StringLength(600)]
        public string About { get; set; }       
        [ForumResourceDisplayName("Address.Label.Email")]
        public string Email { get; set; }
        [ForumResourceDisplayName("Address.Label.Line")]
        public string Line { get; set; }
        [ForumResourceDisplayName("Address.Title")]
        public string Address { get; set; } 
       // [DisplayName("Web / Url")]
        [ForumResourceDisplayName("Address.Web")]
        public string Link { get; set; }
        [ForumResourceDisplayName("Link / Text")]
        public string LinkText { get; set; }
        public Package Package { get; set; }
        [HiddenInput]
        public Guid PackageId { get; set; }
        public AdStatus AdStatus { get; set; }      
        [ForumResourceDisplayName("Ad.Label.Reminder")]
        public bool EnableReminder { get; set; }
		// Edit Properties
		[HiddenInput]
        public Guid Id { get; set; }
        [ForumResourceDisplayName("Image.Label")]
        public HttpPostedFileBase Image { get; set; }
        public string Img { get; set; }
        public bool IsTrial { get; set; }
        [ForumResourceDisplayName("Ad.Label.Duration")]
        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
    }
    public class AdListViewModel
    {
        public IList<Advertisement> Advertisements { get; set; }
        public bool UserCanCreatAd { get; set; }
        public bool TrialAdEnabled { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }
    public class AdViewModel
    {
        public Advertisement Advertisement { get; set; }
        public bool hasVideo { get; set; }
        public bool hasMap { get; set; }
    }
    public class VideoViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] Data { get; set; }     
    }
    public class UploadAdFileViewModel
    {
        public HttpPostedFileBase[] Files { get; set; }
        public Guid advertId { get; set; }
    }
}