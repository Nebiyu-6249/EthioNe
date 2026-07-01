using System;
using System.Collections.Generic;
using System.Linq;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Utilities;
namespace EthioNegarit.Core.Domain.Entities
{
    public partial class ChatRoom : Entity
    {
        public ChatRoom()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PageTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastActive { get; set; }
        public string Slug { get; set; }
        public string MetaDescription { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public bool IsLocked { get; set; }
        public bool? Freeze { get; set; }
        public virtual IList<ChatMessage> ChatMessage { get; set; }
        public virtual IList<ChatRoomMember> ChatRoomMembers { get; set; }       
        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.ChatRoom, Slug);
        public int RoomMessageCount
        {
            get { return ChatMessage.Count(); }
        }
        public int RoomMembersCount
        {
            get { return ChatRoomMembers.Count(); }
        }
    }
}
