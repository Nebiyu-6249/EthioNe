using EthioNegarit.Core.Domain;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class ListLogViewModel
    {
        public IList<LogEntry> LogFiles { get; set; }
    }
}