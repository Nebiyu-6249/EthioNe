using System;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class AttachFileToPostViewModel
    {
        public HttpPostedFileBase[] Files { get; set; }
        public Guid UploadPostId { get; set; }
    }
}