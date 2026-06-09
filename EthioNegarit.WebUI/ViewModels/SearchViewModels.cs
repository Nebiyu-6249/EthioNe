using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class SearchViewModel
    {
        public List<PostViewModel> Posts { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public string Term { get; set; }

    }
}