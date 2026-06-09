using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class CreateDbViewModel
    {
        public bool IsUpgrade { get; set; }
        public string PreviousVersion { get; set; }
        public string CurrentVersion { get; set; }
    }
}