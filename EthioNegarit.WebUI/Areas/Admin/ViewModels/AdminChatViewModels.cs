
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class ListChatRoomsViewModel
    {
        public IEnumerable<ChatRoom> ChatRooms { get; set; }
    }

    public class AdminChatRoomViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("ChatRoom Name")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("ChatRoom Description")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }

        [DisplayName("ChatRoom Colour")]
        [UIHint(AppConstants.EditorTemplateColourPicker), AllowHtml]
        public string Colour { get; set; }

        [DisplayName("Lock The ChatRoom")]
        public bool IsLocked { get; set; }

        [DisplayName("Freeze")]
        public bool Freeze { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("ChatRoom Image")]
        public HttpPostedFileBase[] Files { get; set; }
        public string Image { get; set; }
    }

    public class DeleteChatRoomViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public ChatRoom ChatRoom { get; set; }

    }
}