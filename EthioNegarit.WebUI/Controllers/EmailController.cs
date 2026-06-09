namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Linq;
    using Core.Domain.Entities;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    public class EmailController : BaseController
    {
        private readonly ITopicNotificationService _topicNotificationService;
       private readonly ICategoryNotificationService _categoryNotificationService;
        private readonly ITagNotificationService _tagNotificationService;
       private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicService;
        private readonly ITopicTagService _topicTagService;
        private readonly IChatService _chatService;
        private readonly IActivityService _activityService;
        //    private readonly IChatRoomNotificationService _chatRoomNotificationService;

        public EmailController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IActivityService activityService,
            ITopicNotificationService topicNotificationService, ICategoryNotificationService categoryNotificationService, ICategoryService categoryService,
            ITopicService topicService, ITopicTagService topicTagService, ITagNotificationService tagNotificationService, IChatService chatService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _topicNotificationService = topicNotificationService;
            _categoryNotificationService = categoryNotificationService;
            _categoryService = categoryService;
            _topicService = topicService;
            _topicTagService = topicTagService;
            _tagNotificationService = tagNotificationService;
            _chatService = chatService;
          _activityService = activityService;
            //_chatRoomNotificationService = chatRoomNotificationService;
        }
        [HttpPost]
        [Authorize]
        public void Subscribe(SubscribeEmailViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Add logic to add subscr
                        var isCategory = subscription.SubscriptionType.Contains("category");
                        var isTag = subscription.SubscriptionType.Contains("tag");
                       var isRoom = subscription.SubscriptionType.Contains("room");
                        var id = subscription.Id;
                        var dbUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                        if (isCategory)
                        {
                            // get the category
                            var cat = _categoryService.Get(id);

                            if (cat != null)
                            {

                                // Create the notification
                                var categoryNotification = new CategoryNotification
                                {
                                    Category = cat,
                                    User = dbUser
                                };
                                //save

                                _categoryNotificationService.Add(categoryNotification);
                            }
                        }
                        else if (isTag)
                        {
                            // get the tag
                            var tag = _topicTagService.Get(id);

                            if (tag != null)
                            {

                                // Create the notification
                                var tagNotification = new TagNotification
                                {
                                    Tag = tag,
                                    User = dbUser
                                };
                                //save

                                _tagNotificationService.Add(tagNotification);
                            }
                        }

                        else if (isRoom)
                        {
                            // get the chatRoom
                            var chatRoom = _chatService.Get(id);
                            var subscribtion = _chatService.GetRoomSubscription(dbUser.Id, chatRoom.Id);

                            if (chatRoom != null && subscribtion == null)
                            {

                                // Create the notification
                                var roomMember = new ChatRoomMember
                                {
                                    ChatRoom = chatRoom,
                                    User = dbUser,
                                    EnableNotification = true,
                                    LogOn = false,
                                    LastActive = DateTime.Now
                                };
                                //save

                                _chatService.AddMember(roomMember);

                                _activityService.ChatRoomJoined(chatRoom,dbUser);
                            }
                         
                        }

                        else
                        {
                            // get the category
                            var topic = _topicService.Get(id);

                            // check its not null
                            if (topic != null)
                            {

                                // Create the notification
                                var topicNotification = new TopicNotification
                                {
                                    Topic = topic,
                                    User = dbUser
                                };
                                //save

                                _topicNotificationService.Add(topicNotification);
                            }
                        }

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }

        [HttpPost]
        [Authorize]
        public void UnSubscribe(UnSubscribeEmailViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Add logic to add subscr
                        var isCategory = subscription.SubscriptionType.Contains("category");
                        var isTag = subscription.SubscriptionType.Contains("tag");
                        var isRoom = subscription.SubscriptionType.Contains("room");
                        var isChatReplay = subscription.SubscriptionType.Contains("chatreplay");
                        var id = subscription.Id;
                        var dbUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        if (isCategory)
                        {
                            // get the category
                            var cat = _categoryService.Get(id);

                            if (cat != null)
                            {
                                // get the notifications by user
                                var notifications = _categoryNotificationService.GetByUserAndCategory(LoggedOnReadOnlyUser, cat, true);

                                if (notifications.Any())
                                {
                                    foreach (var categoryNotification in notifications)
                                    {
                                        // Delete
                                        _categoryNotificationService.Delete(categoryNotification);
                                    }
                                }

                            }
                        }
                        else if (isTag)
                        {
                            // get the tag
                            var tag = _topicTagService.Get(id);

                            if (tag != null)
                            {
                                // get the notifications by user
                                var notifications = _tagNotificationService.GetByUserAndTag(LoggedOnReadOnlyUser, tag, true);

                                if (notifications.Any())
                                {
                                    foreach (var n in notifications)
                                    {
                                        // Delete
                                        _tagNotificationService.Delete(n);
                                    }
                                }
                            }
                        }

                        ///////////////////
                        else if (isRoom)
                        {
                            // get the chatRoom
                            var chatRoom = _chatService.Get(id);
                         
                           var subscribed = _chatService.GetRoomSubscription(dbUser.Id, chatRoom.Id);

                            _chatService.DeleteSubscription(subscribed);

                           
                        }

                        ////////////
                        else
                        {
                            // get the topic
                            var topic = _topicService.Get(id);

                            if (topic != null)
                            {
                                // get the notifications by user
                                var notifications = _topicNotificationService.GetByUserAndTopic(LoggedOnReadOnlyUser, topic, true);

                                if (notifications.Any())
                                {
                                    foreach (var topicNotification in notifications)
                                    {
                                        // Delete
                                        _topicNotificationService.Delete(topicNotification);
                                    }
                                }

                            }
                        }

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }
    }
}