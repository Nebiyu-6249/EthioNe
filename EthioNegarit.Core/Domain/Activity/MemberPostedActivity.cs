using EthioNegarit.Core.Domain.Entities;
using System;

namespace EthioNegarit.Core.Domain.Activity
{
 public class MemberPostedActivity : ActivityBase
    {
        public const string KeyPostId = @"PostID";
        public Post Post { get; set; }

        public MemberPostedActivity(Activity activity, Post post)
        {
            ActivityMapped = activity;
            Post = post;
        }

        public static Activity GenerateMappedRecord(Post post, DateTime modified)
        {
            return new Activity
            {
                Data = KeyPostId + Equality + post.Id,
                Timestamp = modified,
                Type = ActivityType.MemberPosted.ToString()
            };

        }
    }
}
