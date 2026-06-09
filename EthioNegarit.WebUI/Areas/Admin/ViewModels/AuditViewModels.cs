using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class AuditRecordsViewModel
    {
        public PagedList<Audit> AuditRecords { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public string Search { get; set; }
    }
}