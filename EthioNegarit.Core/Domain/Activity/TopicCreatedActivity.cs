using EthioNegarit.Core.Domain.Entities;
using System;

namespace EthioNegarit.Core.Domain.Activity
{
   public class TopicCreatedActivity : ActivityBase
    {
        public const string KeyTopicId = @"TopicID";
        public Topic  Topic { get; set; }

        public TopicCreatedActivity(Activity activity, Topic topic)
        {
            ActivityMapped = activity;
            Topic = topic;
        }

        public static Activity GenerateMappedRecord(Topic topic, DateTime modified)
        {
            return new Activity
            {
                Data = KeyTopicId + Equality + topic.Id,
                Timestamp = modified,
                Type = ActivityType.TopicCreated.ToString()
            };

        }
    }
}
