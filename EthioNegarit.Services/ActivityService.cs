namespace EthioNegarit.Services
{
    using Core.Constants;
    using Core.Domain.Activity;
    using Core.Domain.Entities;
    using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
 
    public partial class ActivityService : IActivityService
    {
        private readonly EthioNegaritContext _context;
        private readonly IBadgeService _badgeService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;
        private readonly IChatService _chatService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActivityService(IBadgeService badgeService,IChatService chatService, ILoggingService loggingService, IEthioNegaritContext context, ICacheService cacheService)
        {
            _badgeService = badgeService;
            _loggingService = loggingService;
            _cacheService = cacheService;
            _chatService = chatService;
            _context = context as EthioNegaritContext;
        }

        #region Private Methods
        /// <summary>
        /// Make a badge activity object from the more generic database activity object
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
       private BadgeActivity GenerateBadgeActivity(Activity activity)
        {
            //System.Reflection.MethodBase.GetCurrentMethod().Name
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateBadgeActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // Get the corresponding badge
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(BadgeActivity.KeyBadgeId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A badge activity record with id '{activity.Id}' has no badge id in its data.");
                    return null;
                }

                var badgeId = dataPairs[BadgeActivity.KeyBadgeId];
                var badge = _badgeService.Get(new Guid(badgeId));

                if (badge == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A badge activity record with id '{activity.Id}' has a badge id '{badgeId}' that is not found in the badge table.");
                    return null;
                }

                var userId = dataPairs[BadgeActivity.KeyUserId];
                var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

                if (user == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A badge activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                    return null;
                }

                return new BadgeActivity(activity, badge, user);
            });
        }

        /// <summary>
        /// Make a profile updated joined activity object from the more generic database activity object
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private ProfileUpdatedActivity GenerateProfileUpdatedActivity(Activity activity)
        {

            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateProfileUpdatedActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(ProfileUpdatedActivity.KeyUserId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A profile updated activity record with id '{activity.Id}' has no user id in its data.");
                    return null;
                }

                var userId = dataPairs[ProfileUpdatedActivity.KeyUserId];
                var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

                if (user == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A profile updated activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                    return null;
                }

                return new ProfileUpdatedActivity(activity, user);
            });

        }

        /// <summary>
        /// Make a member joined activity object from the more generic database activity object
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private MemberJoinedActivity GenerateMemberJoinedActivity(Activity activity)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateMemberJoinedActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(MemberJoinedActivity.KeyUserId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A member joined activity record with id '{activity.Id}' has no user id in its data.");
                    return null;
                }

                var userId = dataPairs[MemberJoinedActivity.KeyUserId];
                var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

                if (user == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A member joined activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                    return null;
                }

                return new MemberJoinedActivity(activity, user);
            });
        }

        private TopicCreatedActivity GenerateTopicCreatedActivity(Activity activity)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateTopicCreatedActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(TopicCreatedActivity.KeyTopicId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A topic created activity record with id '{activity.Id}' has no topic id in its data.");
                    return null;
                }

                var topicId = dataPairs[TopicCreatedActivity.KeyTopicId];
                var topic = _context.Topic.FirstOrDefault(x => x.Id == new Guid(topicId));

                if (topic == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A topic created activity record with id '{activity.Id}' has a topic id '{topicId}' that is not found in the user table.");
                    return null;
                }

                return new TopicCreatedActivity(activity, topic);
            });
        }
        private MemberPostedActivity GenerateMemberPostedActivity(Activity activity)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateMemberPostedActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(MemberPostedActivity.KeyPostId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A member posted activity record with id '{activity.Id}' has no post id in its data.");
                    return null;
                }

                var postId = dataPairs[MemberPostedActivity.KeyPostId];
                var post = _context.Post.FirstOrDefault(x => x.Id == new Guid(postId));

                if (post == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A member posted activity record with id '{activity.Id}' has a post id '{postId}' that is not found in the user table.");
                    return null;
                }

                return new MemberPostedActivity(activity, post);
            });
        }

        private ChatRoomActivity GenerateChatRoomActivity(Activity activity)
        {
            //System.Reflection.MethodBase.GetCurrentMethod().Name
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GenerateChatRoomActivity-", activity.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // Get the corresponding chatroom
                var dataPairs = ActivityBase.UnpackData(activity);

                if (!dataPairs.ContainsKey(ChatRoomActivity.KeyRoomId))
                {
                    // Log the problem then skip
                    _loggingService.Error($"A chatRoom activity record with id '{activity.Id}' has no ChatRoom id in its data.");
                    return null;
                }

                var roomId = dataPairs[ChatRoomActivity.KeyRoomId];
                var room = _chatService.Get(new Guid(roomId));

                if (room == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A ChatRoom activity record with id '{activity.Id}' has a ChatRoom id '{roomId}' that is not found in the badge table.");
                    return null;
                }

                var userId = dataPairs[ChatRoomActivity.KeyUserId];
                var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

                if (user == null)
                {
                    // Log the problem then skip
                    _loggingService.Error($"A ChatRoom activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                    return null;
                }

                return new ChatRoomActivity(activity,room, user);
            });
        }
        /// <summary>
        /// Converts a paged list of generic activities into a paged list of more specific activity instances
        /// </summary>
        /// <param name="activities">Paged list of activities where each member may be a specific activity instance e.g. a profile updated activity</param>
        /// <param name="pageIndex"> </param>
        /// <param name="pageSize"> </param>
        /// <returns></returns>
        private PagedList<ActivityBase> ConvertToSpecificActivities(PagedList<Activity> activities, int pageIndex, int pageSize, int total)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "ConvertToSpecificActivities-", activities.GetHashCode(), "-", pageIndex, "-", pageSize,"-",total);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var listedResults = ConvertToSpecificActivities(activities);
                /*   return new PagedList<ActivityBase>(listedResults, pageIndex, pageSize, activities.Count);*/
                return new PagedList<ActivityBase>(listedResults, pageIndex, pageSize, total);
            });
        }

        /// <summary>
        /// Converts a paged list of generic activities into a list of more specific activity instances
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        private IEnumerable<ActivityBase> ConvertToSpecificActivities(IEnumerable<Activity> activities)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "ConvertToSpecificActivities-", activities.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var listedResults = new List<ActivityBase>();
                foreach (var activity in activities)
                {
                    if (activity.Type == ActivityType.BadgeAwarded.ToString())
                    {
                        var badgeActivity = GenerateBadgeActivity(activity);

                        if (badgeActivity != null)
                        {
                            listedResults.Add(badgeActivity);
                        }

                    }
                    else if (activity.Type == ActivityType.MemberJoined.ToString())
                    {
                        var memberJoinedActivity = GenerateMemberJoinedActivity(activity);

                        if (memberJoinedActivity != null)
                        {
                            listedResults.Add(memberJoinedActivity);
                        }
                    }
                    else if (activity.Type == ActivityType.ProfileUpdated.ToString())
                    {

                        var profileUpdatedActivity = GenerateProfileUpdatedActivity(activity);

                        if (profileUpdatedActivity != null)
                        {
                            listedResults.Add(profileUpdatedActivity);
                        }
                    }
                    else if (activity.Type == ActivityType.TopicCreated.ToString())
                    {

                        var topicCreatedActivity = GenerateTopicCreatedActivity(activity);

                        if (topicCreatedActivity != null)
                        {
                            listedResults.Add(topicCreatedActivity);
                        }
                    }
                    else if (activity.Type == ActivityType.MemberPosted.ToString())
                    {

                        var memberPostedActivity = GenerateMemberPostedActivity(activity);

                        if (memberPostedActivity != null)
                        {
                            listedResults.Add(memberPostedActivity);
                        }
                    }
                    else if (activity.Type == ActivityType.ChatRoomJoined.ToString())
                    {

                        var chatRoomActivity = GenerateChatRoomActivity(activity);

                        if (chatRoomActivity != null)
                        {
                            listedResults.Add(chatRoomActivity);
                        }
                    }
                }
                return listedResults;
            });
        }
        #endregion

        /// <summary>
        /// Gets a paged list of badges
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// 
        /// 
        // public PagedList<ActivityBase> GetPagedGroupedActivities(int pageIndex, int pageSize, int amountToTake)
        public PagedList<ActivityBase> GetPagedGroupedActivities(int pageIndex, int pageSize)
        {
            // Read the database for all activities and convert each to a more specialised activity type
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GetPagedGroupedActivities-", pageIndex, "-", pageSize);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {     
                var total = _context.Activity.AsNoTracking().Count();          
                var results = _context.Activity
                      .AsNoTracking()
                      .OrderByDescending(x => x.Timestamp)
                     .Skip((pageIndex - 1) * pageSize)
                     .Take(pageSize)
                    .ToList();
                var  activities = new PagedList<Activity>(results, pageIndex, pageSize, total);             
                var specificActivities = ConvertToSpecificActivities(activities, pageIndex, pageSize,total);
                return specificActivities;
            });

        }

        /// <summary>
        /// Gets all activities by search data field for a Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IEnumerable<Activity> GetDataFieldByGuid(Guid guid)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GetDataFieldByGuid-", guid);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Activity.Where(x => x.Data.Contains(guid.ToString())));
        }

        public PagedList<ActivityBase> SearchPagedGroupedActivities(string search, int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "SearchPagedGroupedActivities-", search, "-", pageIndex, "-", pageSize);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // Read the database for all activities and convert each to a more specialised activity type
                search = StringUtils.SafePlainText(search);
                var totalCount = _context.Activity.Count(x => x.Type.ToUpper().Contains(search.ToUpper()));

                // Get the topics using an efficient
                var results = _context.Activity
                      .Where(x => x.Type.ToUpper().Contains(search.ToUpper()))
                      .OrderByDescending(x => x.Timestamp)
                      .Skip((pageIndex - 1) * pageSize)
                      .Take(pageSize)
                      .ToList();

                // Return a paged list
                var activities = new PagedList<Activity>(results, pageIndex, pageSize, totalCount);

                // Convert
              //  var specificActivities = ConvertToSpecificActivities(activities, pageIndex, pageSize);
                var specificActivities = ConvertToSpecificActivities(activities, pageIndex, pageSize, totalCount);

                return specificActivities;
            });
        }

        public IEnumerable<ActivityBase> GetAll(int howMany)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "GetAll-", howMany);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var activities = _context.Activity.Take(howMany);
                var specificActivities = ConvertToSpecificActivities(activities);
                return specificActivities;
            });
        }

        /// <summary>
        /// New badge has been awarded
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="user"> </param>
        /// <param name="timestamp"> </param>
        public void BadgeAwarded(Badge badge, MembershipUser user, DateTime timestamp)
        {
            var badgeActivity = BadgeActivity.GenerateMappedRecord(badge, user, timestamp);
            Add(badgeActivity);
        }

        /// <summary>
        /// New member has joined
        /// </summary>
        /// <param name="user"></param>
        public void MemberJoined(MembershipUser user)
        {
            var memberJoinedActivity = MemberJoinedActivity.GenerateMappedRecord(user);
            Add(memberJoinedActivity);
        }

        /// <summary>
        /// Profile has been updated
        /// </summary>
        /// <param name="user"></param>
        public void ProfileUpdated(MembershipUser user)
        {
            var profileUpdatedActivity = ProfileUpdatedActivity.GenerateMappedRecord(user, DateTime.UtcNow);
            Add(profileUpdatedActivity);
        }

        public void TopicCreated(Topic topic)
        {
            var topicCreatedActivity = TopicCreatedActivity.GenerateMappedRecord(topic, DateTime.UtcNow);
            Add(topicCreatedActivity);
        }

        public void MemberPosted(Post post)
        {
            var memberPostedActivity = MemberPostedActivity.GenerateMappedRecord(post, DateTime.UtcNow);
            Add(memberPostedActivity);
        }

        public void ChatRoomJoined(ChatRoom chatRoom, MembershipUser user)
        {
            var chatRoomActivity = ChatRoomActivity.GenerateMappedRecord(chatRoom, user, DateTime.UtcNow);
            Add(chatRoomActivity);
        }
        /// <summary>
        /// Delete a list of activities
        /// </summary>
        /// <param name="activities"></param>
        public void Delete(IList<Activity> activities)
        {
            foreach (var activity in activities)
            {
                Delete(activity);
            }
        }

        public Activity Add(Activity newActivity)
        {
            return _context.Activity.Add(newActivity);
        }
        public Activity Get(Guid id)
        {
            var cacheKey = string.Concat(CacheKeys.Activity.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Activity.FirstOrDefault(x => x.Id == id));
        }
        public void Delete(Activity item)
        {
            _context.Activity.Remove(item);
        }       
    }
}
