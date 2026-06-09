using EthioNegarit.Core.Domain.Entities;

namespace EthioNegarit.Core.Domain.Activity
{
    // Seal this class to avoid "virtual member call in constructor" problem
    public sealed class MemberJoinedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";

        public MembershipUser User { get; set; }

        /// <summary>
        /// Constructor - useful when constructing a Member Joined Activity after reading database
        /// </summary>
        public MemberJoinedActivity(Activity activity, MembershipUser user)
        {
            ActivityMapped = activity;
            User = user;
        }

        public static Activity GenerateMappedRecord(MembershipUser user)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id,
                Timestamp = user.CreateDate,
                Type = ActivityType.MemberJoined.ToString()
            };

        }
    }
}
