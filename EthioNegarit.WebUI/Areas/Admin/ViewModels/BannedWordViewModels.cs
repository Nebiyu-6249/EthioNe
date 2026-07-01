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
    public class BannedWordListViewModel
    {
        public PagedList<BannedWord> Words { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public string Search { get; set; }
    }

    public class AddBannedWordViewModel
    {
        [Required]
        public string Word { get; set; }
        public bool IsStopWord { get; set; }
    }

    public class AjaxEditWordViewModel
    {
        [Required]
        public Guid WordId { get; set; }

        [Required]
        public string NewName { get; set; }
    }
}