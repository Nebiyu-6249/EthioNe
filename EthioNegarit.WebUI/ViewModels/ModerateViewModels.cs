using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class ModerateViewModel
    {
        public IList<Topic> Topics { get; set; }
        public IList<Post> Posts { get; set; }
        public bool IsAjaxRequest { get; set; }
    }

    public class ModerateActionViewModel
    {
        public bool IsApproved { get; set; }
        public Guid TopicId { get; set; }
        public Guid PostId { get; set; }
    }
}