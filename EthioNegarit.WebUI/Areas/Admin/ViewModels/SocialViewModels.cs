using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class SocialSettingsViewModel
    {
        [DisplayName("Enable Social Logins")]
        public bool EnableSocialLogins { get; set; }
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }
        public string GooglePlusAppId { get; set; }
        public string GooglePlusAppSecret { get; set; }
        public string MicrosoftAppId { get; set; }
        public string MicrosoftAppSecret { get; set; }
    }
}