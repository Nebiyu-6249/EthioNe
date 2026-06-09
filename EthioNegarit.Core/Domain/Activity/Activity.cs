using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Utilities;
using System;
namespace EthioNegarit.Core.Domain.Activity
{
   public enum ActivityType
    {
        BadgeAwarded,
        MemberJoined,
        ProfileUpdated,
        TopicCreated,
        MemberPosted,
        ChatRoomJoined,
    }

    public class Activity : Entity
    {
        public Activity()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
