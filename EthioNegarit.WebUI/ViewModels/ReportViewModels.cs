using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.WebUI.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EthioNegarit.WebUI.ViewModels
{
    public class ReportViewModel
        {
        public Report Report { get; set; }

        // public List<MembershipUser> Reporters { get; set; }
        // public List<PostViewModel> PostReported { get; set; }
        public List<Post> PostReported { get; set; }
        // public List<PostReportViewModel> PostReports { get; set; }

        //   public List<MessageReportViewModel> MessageReports { get; set; }
        }

 /*   public class PostReportViewModels
        {
        public List<Post> ReportedPosts { get; set; }

        }*/

    public class MemberReportViewModel
        {
        public Guid Id { get; set; }
        public string Username { get; set; }
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(6000, MinimumLength = 4)]
        [ForumResourceDisplayName("Report.Reason")]
        public string Reason { get; set; }
        }

    public class CreatePostReportViewModel
        {
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(6000, MinimumLength = 4)]
        public string Reason { get; set; }
        public Guid ReportOnUserId { get; set; }
        public Guid PostId { get; set; }
        public string PostCreatorUsername { get; set; }

        }
    public class PostReportsViewModel
        {
        public PostViewModel Post { get; set; }
        public List<Report> Reports { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public Guid PostId { get; set; }

        }
    }
