using EthioNegarit.Core.Domain.General;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class ListBadgesViewModel
    {
        public PagedList<Core.Domain.Entities.Badge> Badges { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public string Search { get; set; }
    }
}