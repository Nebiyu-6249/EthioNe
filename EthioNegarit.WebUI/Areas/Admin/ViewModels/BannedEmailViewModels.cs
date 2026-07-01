using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class BannedEmailListViewModel
    {
        public PagedList<BannedEmail> Emails { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public string Search { get; set; }
    }

    public class AddBannedEmailViewModel
    {
        [Required]
        public string Email { get; set; }
    }

    public class AjaxEditEmailViewModel
    {
        [Required]
        public Guid EmailId { get; set; }

        [Required]
        public string NewName { get; set; }
    }
}