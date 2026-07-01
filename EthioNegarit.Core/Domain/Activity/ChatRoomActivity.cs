using System;
using EthioNegarit.Core.Domain.Entities;

namespace EthioNegarit.Core.Domain.Activity
{
   public class ChatRoomActivity : ActivityBase
    {
        public const string KeyRoomId = @"RoomID";
        public const string KeyUserId = @"UserID";

        public ChatRoom ChatRoom { get; set; }
        public MembershipUser User { get; set; }

        /// <summary>
        /// Constructor - useful when constructing a ChatRoom activity after reading database
        /// </summary>
        public ChatRoomActivity(Activity activity, ChatRoom chatRoom, MembershipUser user)
        {
            ActivityMapped = activity;
            ChatRoom = chatRoom;
            User = user;
        }

        public static Activity GenerateMappedRecord(ChatRoom chatRoom, MembershipUser user, DateTime timestamp)
        {
            return new Activity
            {
                // badge=badgeId,user=userId
                Data = KeyRoomId + Equality + chatRoom.Id + Separator + KeyUserId + Equality + user.Id,
                Timestamp = timestamp,
                Type = ActivityType.ChatRoomJoined.ToString()
            };

        }
    }
}
