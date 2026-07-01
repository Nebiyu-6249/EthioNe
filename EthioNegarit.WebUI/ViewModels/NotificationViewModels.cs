using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace EthioNegarit.WebUI.ViewModels
{    public class NotificationsViewModel
    {
        public IList<Topic> Topics { get; set; }
        public IList<Post> Posts { get; set; }
        public MembershipUser LoggedOnUser { get; set; }
        public int NewNotification { get; set; }
        public int ModerateCount { get; set; }
    }
    public class MemberNotificationViewModel
    {
        public string Subject { get; set; }
        public string Notice { get; set; }
     //   public Uri Url { get; set; }
    }
}