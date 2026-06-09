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
    using Core.Domain;
    using Core.Domain.Entities;
    using MembershipUser = Core.Domain.Entities.MembershipUser;
    using Core.Domain.Enums;
    using Hubs;
    using Microsoft.AspNet.SignalR;
    using Newtonsoft.Json;
    public partial class ChatController : BaseController
        {
        private readonly IRoleService _roleService;
        private readonly IChatService _chatService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly IBannedWordService _bannedWordService;
        static ChatModel chatModel;
        public ChatHub chatHub;
        //   private readonly IHubContext<ChatHub> _hubContext;
        public ChatController (ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
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
       /* private enum ChatReactionType
            {
            Positive,
            Negative,
            };*/
        public ActionResult Index ()
            {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                var rooms = _chatService.GetAll();
                var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanToFreeze);
                var roomsToUpdate = new List<ChatRoom>();
                roomsToUpdate.AddRange(rooms);
                foreach (var room in roomsToUpdate)
                    {
                    if (room.LastActive > date)
                        {
                        room.Freeze = true;
                        }
                    }
                // Get the  View Models
                var chatRooms = ViewModelMapping.CreateChatRoomViewModels(rooms, LoggedOnReadOnlyUser, SettingsService.GetSettings());
                try
                    {
                    unitOfWork.Commit();
                    }
                catch (Exception ex)
                    {
                    LoggingService.Error(ex);
                    }
                var viewModel = new ChatRoomsViewModel
                    {
                    ChatRooms = chatRooms
                    };

                if (Request.IsAjaxRequest())
                    {
                    return PartialView("_chatRooms", viewModel);
                    }
                return View(viewModel);
                }
            }
        [HttpPost]
        public PartialViewResult AjaxMoreMessages (ChatMessagesViewModel getMoreMessagesViewModel)
            {
            using (UnitOfWorkManager.NewUnitOfWork())
                {
                // Get Chat Room
                var chatRoom = _chatService.GetChatRoom(getMoreMessagesViewModel.RoomId);
                var roomMessages = getMoreMessagesViewModel.MyChat == true ? _chatService.GetUserMessages(getMoreMessagesViewModel.PageIndex, SiteConstants.Instance.ChatMessageListSize, int.MaxValue, chatRoom.Id, LoggedOnReadOnlyUser.Id) : _chatService.GetRoomMessages(getMoreMessagesViewModel.PageIndex, SiteConstants.Instance.ChatMessageListSize, int.MaxValue, chatRoom.Id);
                if (getMoreMessagesViewModel.MyChat == true)
                {
                    foreach (var roomMessage in roomMessages)
                    {
                        var messagesReplies = _chatService.GetReplies(roomMessage.Id).ToList();
                        roomMessages.AddRange(messagesReplies.ToList());
                    }
                }
                var chatMessages = ViewModelMapping.CreateChatMessagesViewModel(chatRoom, roomMessages.ToList(), roomMessages.PageIndex, roomMessages.TotalCount, roomMessages.TotalPages, LoggedOnReadOnlyUser);
                var viewModel = new ChatMessagesViewModel
                    {
                    ChatMessages = chatMessages.ChatMessages,
                    ChatRoom = chatMessages.ChatRoom,
                    MyChat = getMoreMessagesViewModel.MyChat,
                    PageIndex = chatMessages.PageIndex,
                    TotalCount = chatMessages.TotalCount,
                    TotalPages = chatMessages.TotalPages,
                    };
               return PartialView(viewModel);
             //  return PartialView("Messages", viewModel);
            }
            }
        public PartialViewResult Messages (Guid roomId,bool myChat)
            {
            if (Request.IsAjaxRequest())
            {
                var logged_on = false;
                using (UnitOfWorkManager.NewUnitOfWork())
                {                
                        // Get Chat Room
                        var chatRoom = _chatService.GetChatRoom(roomId);
                      /*  if (chatRoom != null)
                        {*/
                            if (User.Identity.IsAuthenticated)
                            {
                                var subscription = _chatService.GetRoomSubscription(LoggedOnReadOnlyUser.Id, chatRoom.Id);
                                if (subscription != null)
                                {
                                 //   logged_on = subscription.LogOn;
                        TimeSpan timespan = DateTime.Now - subscription.LastActive;
                        if (timespan.TotalSeconds < 30 && subscription.LogOn == true)
                        {
                            logged_on = true;
                        }
                    }
                    
                            }
                            var roomMessages = myChat == true ? _chatService.GetUserMessages(1, SiteConstants.Instance.ChatMessageListSize, int.MaxValue, chatRoom.Id, LoggedOnReadOnlyUser.Id) : _chatService.GetRoomMessages(1, SiteConstants.Instance.ChatMessageListSize, int.MaxValue, chatRoom.Id);
                            if (myChat == true)
                            {
                        foreach (var roomMessage in roomMessages.ToList())
                                {
                                    var messagesReplies = _chatService.GetReplies(roomMessage.Id).ToList();
                            roomMessages.AddRange(messagesReplies.ToList());
                        }
                    }
                            var chatMessages = ViewModelMapping.CreateChatMessagesViewModel(chatRoom, roomMessages.ToList(), 1, roomMessages.TotalCount, roomMessages.TotalPages, LoggedOnReadOnlyUser);
                            var viewModel = new ChatMessagesViewModel
                            {
                                ChatMessages = chatMessages.ChatMessages,
                                ChatRoom = chatMessages.ChatRoom,
                                PageIndex = chatMessages.PageIndex,
                                TotalCount = chatMessages.TotalCount,
                                TotalPages = chatMessages.TotalPages,
                                LogOn = logged_on,
                              ChatEnabled = ChatEnabled()
                };              
                    return PartialView(viewModel);
                }
            }
			return null;
			}
     //   [Authorize]
        public PartialViewResult CreateMessage (Guid roomId, Guid? messageId)
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
                    };
                if (message_Id != null)
                    {
                    var chatMessage = _chatService.GetMessage(message_Id);
                    viewModel.IsReplay = true;
                    viewModel.InReplyTo = messageId;
                    viewModel.ReplyToUsername = chatMessage.User.UserName;
                    }
                else
                    {
                    viewModel.IsReplay = false;
                    viewModel.InReplyTo = Guid.Empty;
                    }
                return PartialView("_createMessage", viewModel);
                }
            }
        [HttpPost]
      //  [Authorize]
        public ActionResult CreateMessage (CreateMessageViewModel model)
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
                        DisableReplay = false,
                        InReplyTo = model.InReplyTo,
                        IsReplay = model.IsReplay
                        };

                    message = _chatService.AddMessage(message);
                    try
                        {
                        unitOfWork.Commit();
                   //    var hubcontext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
              //         hubcontext.Clients.All.addNewMessageToPage(model.Message);
                    }
                    catch
                        {
                        unitOfWork.Rollback();              
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
 var viewModel = ViewModelMapping.CreateMessageViewModel(message, new List<ChatResponse>(), LoggedOnReadOnlyUser);
                    //  IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    //    chatHub = new ChatHub();
                    //   chatHub.Send(message);
                    var hubcontext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                      hubcontext.Clients.All.addNewMessageToPage(model.Message);
                    //   var messageToReturn = JsonConvert.SerializeObject(viewModel);
                    //   hubcontext.Clients.All.addNewMessageToPage(messageToReturn);
                    return PartialView("_chatMessage", viewModel);
                }
            }
            return null;
            }
        [ChildActionOnly]
    //    [Authorize]
        public PartialViewResult GetSubscribedRooms ()
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
                       // ShowUnSubscribedLink = true
                        ShowUnSubscribedLink = UserIsAuthenticated && (_chatService.GetRoomSubscription(loggedOnUser, chatRoom).Any())
						};
                    viewModel.Add(model);
                    }
                return PartialView(viewModel);
               }
            }
        [HttpPost]
       // [Authorize]
        public void LogOn (Guid roomId, bool logOn)
            {
          //  chatModel.UserId = Guid.Empty;
            chatModel.LogOn = logOn;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                  //  if (User.Identity.IsAuthenticated)
					//{
					var subscription = _chatService.GetRoomSubscription(LoggedOnReadOnlyUser.Id, roomId);
                    if (subscription != null)
                    {
                    //    subscription.LogOn = logOn;
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
				catch(Exception ex)
					{
					unitOfWork.Rollback();
					LoggingService.Error(ex);
				 throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
					}

				}
			
			}
     //   [Authorize]
        public ActionResult Lobby (string slug)
            {
            if (chatModel == null) chatModel = new ChatModel();
            chatModel.IsSubscribed = false;
           chatModel.UserId = Guid.Empty;
            chatModel.LogOn = false;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
				{
				// Get the ChatRoom
				var chatRoom = _chatService.GetChatRoom(slug);
				chatModel.ChatRoom = chatRoom;
						if(chatRoom != null)
							{
							//Update ChatRoom
							chatRoom.LastActive = DateTime.UtcNow;
							chatRoom.Freeze = false;
						/*	RecentActivity recentActivity = (RecentActivity)Session["RecentActivity"];
							if(recentActivity == null)
								{
								recentActivity = new RecentActivity();
								Session["RecentActivity"] = recentActivity;
								}
							recentActivity.AddRecentChatroomVisited(chatRoom);
                    */
							if(User.Identity.IsAuthenticated)
								{
                        chatModel.chatEnabled = ChatEnabled();
                                var subscription = _chatService.GetRoomSubscription(LoggedOnReadOnlyUser.Id, chatRoom.Id);
								if(subscription != null)
									{                           
                            chatModel.IsSubscribed = true;
                          TimeSpan timespan = DateTime.Now - subscription.LastActive;
                        if (timespan.TotalSeconds < 30 && subscription.LogOn == true)
                        {
                            chatModel.LogOn = true;
                            chatModel.UserId = subscription.User.Id;
                        }
                        }
                   }
							
				try
					{
					unitOfWork.Commit();
					}
				catch(Exception ex)
					{				
					unitOfWork.Rollback();
					LoggingService.Error(ex);
					throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
					}
                    return View(chatModel);
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        [HttpPost]
      //  [Authorize]
        public void Report (ChatResponseViewModel chatResponseViewModel)
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
                        messageWriter.DisableChat = true;
                        messageWriter.SpamCount = messageWriter.SpamCount + 1;
                        }
                    if (messageWriter.SpamCount > SiteConstants.Instance.SpamCountToBan)
                        {
                        messageWriter.IsBanned = true;
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
        [HttpPost]
    //  [Authorize]
        public void Like (ChatResponseViewModel chatResponseViewModel)
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
        private void MarkMemberReaction (ChatMessage chatMessage, MembershipUser messageWriter, MembershipUser responder, ChatReaction chatReactionType)
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
        public ActionResult RoomMembers (Guid roomId, int? p, int? amountToShow)
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
        public void Delete (DeleteChatMessageViewModel deleteChatMessageViewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {

                    var chatMessage = _chatService.GetMessage(deleteChatMessageViewModel.Id);
                    //   var has_reponse = false;
                    //  var has_reponse = chatMessage.ChatResponses.Any();

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
                        // Check to delete completely
                        /*  if (chatMessage.IsReplay == false || chatMessage.InReplyTo == null) {
                              //else if message has not replay message or chat Response delete
                              if (has_reponse == false || messageReplies.Count <= 0)
                              {
                                  _chatService.Delete(chatMessage);
                              }
                          }                             
                          //else it has replay msg or Response remove message content and mark as removed //is removed true                        
                         else if (has_reponse == true || messageReplies.Count > 0)
                          {
                              chatMessage.Content = "Deleted";
                              chatMessage.Deleted = true;
                          }
                          */
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
                    var hubcontext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubcontext.Clients.All.addNewMessageToPage("Message");
                }
            }
        }
        public bool ChatEnabled()
        {
            var chatEnabled = false;
            if(User.Identity.IsAuthenticated){
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
    }
    }

/*  public static void BroadcastChatMessage(List<ChatReplay> dataList)
        {
            // var dataList = db.Customers.ToList();

            // ChatHub.ChatMessageBroadcast(dataList);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            context.Clients.All.getUpdateData(dataList);

        }*/
