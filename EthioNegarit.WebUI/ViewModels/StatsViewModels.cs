using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class MainStatsViewModel
    {
        public int PostCount { get; set; }
        public int TopicCount { get; set; }
        public int MemberCount { get; set; }
        public int CategoryCount { get; set; }
        public IList<MembershipUser> LatestMembers { get; set; }
    }
}