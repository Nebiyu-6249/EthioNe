namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    using System.Web.Mvc;
    using ViewModels.Mapping;
    using Core.Constants;
    using System.Web.Security;
    using Core.Domain.Entities;
    using MembershipUser = Core.Domain.Entities.MembershipUser;
    using Core.Domain.Enums;
    using Microsoft.AspNet.SignalR;
    using Hubs;
    using Areas.Admin.ViewModels;

    public partial class ChatController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IChatService _chatService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly IBannedWordService _bannedWordService;
        static ChatModel chatModel;
        public ChatController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
          IMembershipService membershipService,
          ILocalizationService localizationService,
          IRoleService roleService,
          ICategoryService categoryService,
          ISettingsService settingsService,
          ITopicService topicService,
          IBannedWordService bannedWordService,
          ICategoryNotificationService categoryNotificationService, ICacheService cacheService, IChatService chatService, IVoteService voteService, IMembershipUserPointsService membershipUserPointsService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _roleService = roleService;
            _chatService = chatService;
            _membershipUserPointsService = membershipUserPointsService;
            _bannedWordService = bannedWordService;
        }
        public ActionResult Index(int? p,bool freezed = false)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                // var rooms = _chatService.GetAll();
                var pageIndex = p ?? 1;
                var rooms = _chatService.GetRooms(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.ActiveTopicsListSize, freezed);               
                // Get the  View Models
                var chatRooms = ViewModelMapping.CreateChatRoomViewModels(rooms, LoggedOnReadOnlyUser, SettingsService.GetSettings());
                var viewModel = new ChatRoomsViewModel
                {
                    ChatRooms = chatRooms,
                    Freezed = freezed,
                    FreezedRoomsCount = _chatService.FreezedRoomsCount(),
                    PageIndex = pageIndex,
                    TotalCount = rooms.TotalCount,
                    TotalPages = rooms.TotalPages
                };
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_chatRooms", viewModel);
                }
                return View(viewModel);
            }
        }
        // [Authorize]
        public ActionResult Room(string slug)
        {
            //Report
            var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
            var chatReports = _chatService.GetReports(LoggedOnReadOnlyUser.Id, datefrom);
            if (chatReports.Count >= SiteConstants.Instance.ReportCountToDisable)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Chat.Disallowed"),
                    MessageType = GenericMessages.danger
                };
                return RedirectToAction("Index", "Chat");
            }
            if (chatModel == null) chatModel = new ChatModel();
            // Get Chat Room
            var chatRoom = _chatService.GetChatRoom(slug);
            if (chatRoom == null)
                return HttpNotFound();
            chatModel.ChatRoom = chatRoom;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        chatModel.UserId = LoggedOnReadOnlyUser.Id;
                        var subscription = _chatService.GetRoomSubscription(LoggedOnReadOnlyUser.Id, chatRoom.Id);
                        if (subscription == null)
                        {
                            var roomMember = new ChatRoomMember
                            {
                                ChatRoom = chatRoom,
                                User = LoggedOnReadOnlyUser,
                                EnableNotification = true,
                                LogOn = true,
                                LastActive = DateTime.Now
                            };
                            _chatService.AddMember(roomMember);
                        }
                        else
                        {
                            subscription.LogOn = true;
                            subscription.LastActive = DateTime.Now;
                        }
                    }
                    else
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.LogonGeneric"),
                            MessageType = GenericMessages.danger
                        };
                        return RedirectToAction("Index", "Chat");
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
            return View(chatModel);
        }
        [HttpPost]
        public PartialViewResult AjaxMoreMessages(ChatMessagesViewModel getMoreMessagesViewModel)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                // Get Chat Room
                var chatRoom = _chatService.GetChatRoom(getMoreMessagesViewModel.RoomId);
                var roomMessages = _chatService.GetRoomMessages(getMoreMessagesViewModel.PageIndex, SiteConstants.Instance.ChatMessagesListSize, int.MaxValue, chatRoom.Id);
                var chatMessages = ViewModelMapping.CreateChatMessagesViewModel(chatRoom, roomMessages.ToList(), LoggedOnReadOnlyUser, settings);
                var viewModel = new ChatMessagesViewModel
                {
                    ChatMessages = chatMessages,
                    ChatRoom = chatRoom,
                    PageIndex = getMoreMessagesViewModel.PageIndex,
                    TotalCount = roomMessages.TotalCount,
                    TotalPages = roomMessages.TotalPages,
                };
                return PartialView(viewModel);
                //  return PartialView("Messages", viewModel);
            }
        }
        public PartialViewResult Messages(Guid roomId)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var settings = SettingsService.GetSettings();
                    // Get Chat Room
                    var chatRoom = _chatService.GetChatRoom(roomId);
                    var roomMessages = _chatService.GetRoomMessages(1, SiteConstants.Instance.ChatMessagesListSize, int.MaxValue, chatRoom.Id);
                    foreach (var roomMessage in roomMessages.ToList())
                    {
                        var messagesReplies = _chatService.GetReplies(roomMessage.Id).ToList();
                        roomMessages.AddRange(messagesReplies.ToList());
                    }
                    var chatMessages = ViewModelMapping.CreateChatMessagesViewModel(chatRoom, roomMessages.ToList(), LoggedOnReadOnlyUser, settings);
                    var viewModel = new ChatMessagesViewModel
                    {
                        ChatMessages = chatMessages,
                        ChatRoom = chatRoom,
                        PageIndex = 1,
                        TotalCount = roomMessages.TotalCount,
                        TotalPages = roomMessages.TotalPages,
                    };
                    return PartialView(viewModel);
                }
            }
            return null;
        }
        //   [Authorize]
        public PartialViewResult CreateMessage(Guid roomId, Guid? messageId)
        {
            var message_Id = messageId ?? Guid.Empty;
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var chatRoom = _chatService.Get(roomId);
                var viewModel = new CreateMessageViewModel
                {
                    User = loggedOnUser,
                    ChatRoom = chatRoom,
                    RoomId = chatRoom.Id,
                    ReplyToUsername = "",
                    IsReplay = false,
                    InReplyTo = Guid.Empty
                };
                if (message_Id != null)
                {
                    var chatMessage = _chatService.GetMessage(message_Id);
                    viewModel.IsReplay = true;
                    viewModel.InReplyTo = chatMessage.Id;
                    viewModel.ReplyToUsername = chatMessage.User.UserName;
                    viewModel.ReplyToUserId = chatMessage.User.Id;
                }
                //  return PartialView("_createMessage", viewModel);
                return PartialView(viewModel);
            }
        }
        [HttpPost]
        //  [Authorize]
        public void CreateMessage(CreateMessageViewModel model)
        {
            if (Request.IsAjaxRequest())
            {
                var bannedWordsList = _bannedWordService.GetAll();
                List<string> bannedWords = null;
                if (bannedWordsList.Any())
                {
                    bannedWords = bannedWordsList.Select(x => x.Word).ToList();
                }
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    var chatRoom = _chatService.Get(model.RoomId);
                    var message = new ChatMessage
                    {
                        ChatRoom = chatRoom,
                        User = loggedOnUser,
                        Content = _bannedWordService.SanitiseBannedWords(model.Message, bannedWords),
                        Timestamp = DateTime.UtcNow,
                        FlaggedAsSpam = false,
                        InReplyTo = model.InReplyTo,
                        IsReplay = model.IsReplay
                    };
                    message = _chatService.AddMessage(message);
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.All.updateMessages();
                }
            }
        }
        [ChildActionOnly]
        //    [Authorize]
        public PartialViewResult GetSubscribedRooms()
        {
            var viewModel = new List<ChatRoomViewModel>();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var chatRooms = loggedOnUser.ChatRoomMembers.Select(x => x.ChatRoom).ToList();
                foreach (var chatRoom in chatRooms)
                {
                    var model = new ChatRoomViewModel
                    {
                        ChatRoom = chatRoom,
                        IsSubscribed = true,
                    };
                    viewModel.Add(model);
                }
                return PartialView(viewModel);
            }
        }
        [HttpPost]
        // [Authorize]
        public void LogOn(Guid roomId, bool logOn)
        {
            //  chatModel.UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                //  chatModel.LogOn = logOn;
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        //  if (User.Identity.IsAuthenticated)
                        //{
                        var subscription = _chatService.GetRoomSubscription(LoggedOnReadOnlyUser.Id, roomId);
                        if (subscription != null)
                        {

                            // chatModel.UserId = LoggedOnReadOnlyUser.Id;
                            if (logOn)
                            {
                                subscription.LogOn = true;
                                subscription.LastActive = DateTime.Now;
                                chatModel.UserId = LoggedOnReadOnlyUser.Id;
                            }
                            else
                            {
                                subscription.LogOn = false;
                                chatModel.UserId = Guid.Empty;
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
        }
        [HttpPost]
        //  [Authorize]
        public void Report(ChatResponseViewModel chatResponseViewModel)
        {
            //1 Like
            //0 DisLike
            //-1 Spam spamreportlink
            if (Request.IsAjaxRequest())
            {
                // Quick check to see if user is locked out, when logged in
                if (LoggedOnReadOnlyUser.IsLockedOut | !LoggedOnReadOnlyUser.IsApproved)
                {
                    FormsAuthentication.SignOut();
                    throw new Exception(LocalizationService.GetResourceString("Errors.NoAccess"));
                }
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    // get the Message
                    var chatMessage = _chatService.GetMessage(chatResponseViewModel.MessageId);
                    // Now get the current user/Reporter
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    // Also get the user that post the chatMessage 
                    var messageWriter = chatMessage.User;
                    // Mark  Report
                    MarkMemberReaction(chatMessage, messageWriter, loggedOnUser, ChatReaction.Negative);
                    var reportCount = chatMessage.ChatResponses.Count(x => x.Amount < 0);
                    if (reportCount > SiteConstants.Instance.ReportCountsToFlaggedAsSpam)
                    {
                        chatMessage.FlaggedAsSpam = true;
                        chatMessage.Content = "Removed";
                        chatMessage.Deleted = true;
                    }
                    // Finally Check Message writer Spam Report count 
                    //Get Reports
                    //Get all member spam post from date 
                    //Get Overall MemberSpamedMessage/
                    var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                    //Monthly Report
                    var spamReports = _chatService.GetReports(messageWriter.Id, datefrom);
                    if (spamReports.Count > SiteConstants.Instance.ReportCountToDisable)
                    {
                        //  messageWriter.DisableChat = true;
                        //   messageWriter.SpamCount = messageWriter.SpamCount + 1;
                    }
                    /*  if (messageWriter.SpamCount > SiteConstants.Instance.SpamCountToBan)
                          {
                          messageWriter.IsBanned = true;
                          }  */
                    try
                    {
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
        }
        [HttpPost]
        //  [Authorize]
        public void Like(ChatResponseViewModel chatResponseViewModel)
        {
            //1 Like
            //0 DisLike
            //-1 Spam spamreportlink
            if (Request.IsAjaxRequest())
            {
                // Quick check to see if user is locked out, when logged in
                if (LoggedOnReadOnlyUser.IsLockedOut | !LoggedOnReadOnlyUser.IsApproved)
                {
                    FormsAuthentication.SignOut();
                    throw new Exception(LocalizationService.GetResourceString("Errors.NoAccess"));
                }
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    // Now get the current user
                    // Get a db user
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                    // Firstly get the Message
                    var chatMessage = _chatService.GetMessage(chatResponseViewModel.MessageId);

                    // Now get the current user
                    var responder = loggedOnUser;

                    // Also get the user that post the chatMessage 
                    var messageWriter = chatMessage.User;


                    MarkMemberReaction(chatMessage, messageWriter, responder, ChatReaction.Positive);
                    try
                    {
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
        }
        private void MarkMemberReaction(ChatMessage chatMessage, MembershipUser messageWriter, MembershipUser responder, ChatReaction chatReactionType)
        {
            //   var settings = SettingsService.GetSettings();
            // Check this user is not the post owner
            if (responder.Id != messageWriter.Id)
            {
                // Not the same person, now check they haven't respond on this message before
                var chatResponses = chatMessage.ChatResponses.Where(x => x.MembershipUserRespond.Id == LoggedOnReadOnlyUser.Id).ToList();

                if (chatResponses.Any())
                {
                    // Already response, so delete the response and remove the points
                    var responsesToDelete = new List<ChatResponse>();
                    responsesToDelete.AddRange(chatResponses);
                    foreach (var response in responsesToDelete)
                    {
                        _chatService.DeleteResponse(response);
                    }
                }
                else
                {
                    // Update the messagee with the new response of the responder
                    var chatResponse = new ChatResponse
                    {
                        ChatMessage = chatMessage,
                        User = messageWriter,
                        Amount = (chatReactionType == ChatReaction.Negative) ? (-1) : (1),
                        MembershipUserRespond = responder,
                        DateResponse = DateTime.UtcNow
                    };
                    _chatService.AddResponse(chatResponse);
                }
            }
        }
        public ActionResult RoomMembers(Guid roomId, int? p, int? amountToShow)
        {
            var pageIndex = p ?? 1;
            var amountShow = amountToShow ?? SiteConstants.Instance.AdminListPageSize;
            var isOnLine = false;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var chatRoom = _chatService.GetChatRoom(roomId);
                var roomMembers = _chatService.GetRoomMembers(roomId, pageIndex, SiteConstants.Instance.AdminListPageSize, amountShow).ToList();
                var members = roomMembers.Select(x => x.User).ToList();
                foreach (var user in roomMembers)
                {
                    if (user.LogOn == true)
                    {
                        /*  var timeNow = DateTime.UtcNow.AddSeconds(-50);
                          if (user.LastActive < timeNow)
                          {
                              user.LogOn = false;
                          }*/
                        TimeSpan span = DateTime.Now - user.LastActive;
                        if (span.TotalSeconds > 30)
                        {
                            user.LogOn = false;
                        }
                    }
                    unitOfWork.SaveChanges();
                }

                var viewModel = new ChatRoomMembersViewModel
                {
                    ChatRoomMembers = roomMembers.ToList(),
                    ChatRoom = chatRoom,
                    RoomMembers = members.ToList(),
                    MemberIsOnline = isOnLine,
                    PageIndex = pageIndex
                };

                try
                {
                    unitOfWork.Commit();
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_roomMembers", viewModel);
                    }
                    else
                    {
                        return View(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                }
            }
            return null;
        }
        [HttpPost]
        public void Delete(DeleteChatMessageViewModel deleteChatMessageViewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var chatMessage = _chatService.GetMessage(deleteChatMessageViewModel.Id);
                    if (chatMessage.User.Id == LoggedOnReadOnlyUser.Id)
                    {
                        var messageReplies = _chatService.GetReplies(chatMessage.Id);
                        if (messageReplies.Any())
                        {
                            chatMessage.Content = "Deleted";
                            chatMessage.Deleted = true;
                            unitOfWork.SaveChanges();
                        }
                        else
                        {
                            // Check to delete completely or not
                            if (chatMessage.ChatResponses.Any())
                            {
                                chatMessage.Content = "Deleted";
                                chatMessage.Deleted = true;
                                unitOfWork.SaveChanges();
                            }
                            else
                            {
                                _chatService.Delete(chatMessage);
                                unitOfWork.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(LocalizationService.GetResourceString("Errors.NoPermission"));
                    }
                    try
                    {
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
        }
        public bool ChatEnabled()
        {
            var chatEnabled = false;
            if (User.Identity.IsAuthenticated)
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                //Get all chat report from date 
                var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                var spamReports = _chatService.GetReports(loggedOnUser.Id, datefrom);
                if (spamReports.Count < SiteConstants.Instance.ReportCountToDisable)
                {
                    chatEnabled = true;
                }
            }
            return chatEnabled;
        }
        private static string GroupName(Guid roomId)
        {
            string room = roomId.ToString();
            return "room-" + room;
        }
    }
}

