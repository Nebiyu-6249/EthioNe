namespace EthioNegarit.WebUI.Controllers
{
    using System.Web.Hosting;
    using Application;
    using Areas.Admin.ViewModels;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Utilities;
    using ViewModels;
    using System.Web;
    using System.Text;
    using System.Collections.Generic;

    public partial class PrivateMessageController : BaseController
        {
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IEmailService _emailService;
        private readonly IConfigService _configService;
        private readonly IAttachmentService _attachmentService;
        public PrivateMessageController (ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IPrivateMessageService privateMessageService, IAttachmentService attachmentService,
            IEmailService emailService, IConfigService configService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
            {
            _privateMessageService = privateMessageService;
            _emailService = emailService;
            _configService = configService;
            _attachmentService = attachmentService;
            }
        [Authorize]
        public ActionResult Index()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                if (loggedOnUser.DisablePrivateMessages == true)
                {
                    if (loggedOnUser.TotalPoints >= AppConstants.PointsToEnableMessaging)
                    {
                        loggedOnUser.DisablePrivateMessages = false;
                      //  return View();
                    }
                    else
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                            MessageType = GenericMessages.danger
                        };
                        return RedirectToAction("Index", "Home");
                    }
                }
                try
                {
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                }
            }
               return View();
        }
        [Authorize]
        public ActionResult Inbox(int? p)
        {
            if (LoggedOnReadOnlyUser.DisablePrivateMessages == true)
            {              
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                    MessageType = GenericMessages.danger
                };
                return RedirectToAction("Index", "Home");
            }
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;
                var newMessagesCount = _privateMessageService.GetNewMessages(LoggedOnReadOnlyUser.Id).DistinctBy(x => x.UserFrom).Count();
                //Get All Messages
                var messages = _privateMessageService.GetUserPrivateMessages(pageIndex, SiteConstants.Instance.PrivateMessageListSize, LoggedOnReadOnlyUser);
                var viewModel = new PrivateMessagesViewModel
                {
                    AllMessages = messages,
                    UnreadMessages = messages.Where(x => x.UnRead == true).ToList(),
                    SentMessages = messages.Where(x => x.IsSentMessage == true).ToList(),
                    ReceivedMessages = messages.Where(x => x.IsReceivedMessage == true).ToList(),
                    StarredMessages = messages.Where(x => x.IsStarred == true).ToList(),
                    NewMessagesCount = newMessagesCount,
                    PageIndex = pageIndex,
                    TotalCount = messages.TotalCount,
                    TotalPages = messages.TotalPages
                };
                return PartialView(viewModel);
            }
        }     
        public ActionResult View(Guid from)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var userFrom = MembershipService.GetUser(from);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);               
                    if (userFrom.Id != LoggedOnReadOnlyUser.Id)
                    {
                        // Mark all messages read sent from the userFrom
                        var unreadMessages = loggedOnUser.PrivateMessagesReceived.Where(x => x.UserFrom.Id == userFrom.Id && !x.IsRead);
                        if (unreadMessages != null)
                        {
                            foreach (var message in unreadMessages)
                            {
                                // Update message as read
                                message.IsRead = true;
                                // Get the sent version and update that too
                                var sentMessage = _privateMessageService.GetMatchingSentPrivateMessage(message.DateSent, message.UserFrom.Id, message.UserTo.Id);
                                if (sentMessage != null)
                                {
                                    sentMessage.IsRead = true;
                                }
                            }
                        }
                        // Commit all changes
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                        }
                        var allMessages = _privateMessageService.GetUsersConversation(1, SiteConstants.Instance.PagingGroupSize, loggedOnUser, userFrom);
                        // Now order them into an order of messages
                        var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
                        var viewModel = new MessagesListViewModel
                        {
                            From = userFrom,
                            PrivateMessages = allMessages,
                            FromUserIsOnline = userFrom.LastActivityDate > date,
                            IsAjaxRequest = Request.IsAjaxRequest(),
                            IsBlocked = loggedOnUser.BlockedUsers.Any(x => x.Blocked.Id == userFrom.Id),
                            Blocked = userFrom.BlockedUsers.Any(x => x.Blocked.Id == loggedOnUser.Id)
                        };
                        return View(viewModel);
                    }
                }
            }
            return null;
        }
        [HttpPost]
        public ActionResult AjaxMore (GetMoreViewModel viewModel)
            {
            using (UnitOfWorkManager.NewUnitOfWork())
                {
                if (Request.IsAjaxRequest())
                    {
                    var userFrom = MembershipService.GetUser(viewModel.UserId);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                    var settings = SettingsService.GetSettings();
                    if (!settings.EnablePrivateMessages || LoggedOnReadOnlyUser.DisablePrivateMessages == true)
                        {
                        return Content(LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }

                    var allMessages = _privateMessageService.GetUsersConversation(viewModel.PageIndex, SiteConstants.Instance.PagingGroupSize, loggedOnUser, userFrom);

                    var partialViewModel = new MessagesListViewModel
                        {
                        From = userFrom,
                        PrivateMessages = allMessages,
                        IsAjaxRequest = Request.IsAjaxRequest(),
                        };

                    return PartialView(partialViewModel);
                    }
                return Content(string.Empty);
                }
            }
        private static string PmAjaxError (string message)
            {
            return $"<p class=\"pmerrormessage\">{message}</p>";
            }
        internal ActionResult ErrorToInbox (string errorMessage)
            {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                Message = errorMessage,
                MessageType = GenericMessages.danger
                };
            // Not allowed in here so
            return RedirectToAction("Index", "PrivateMessage");
            }
        [HttpPost]
        [Authorize]
        public ActionResult MessageStar (MarkPrivateMessageViewModel viewModel)
            {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                if (Request.IsAjaxRequest())
                    {
                    var privateMessage = _privateMessageService.Get(viewModel.Id);
                    if (privateMessage.IsStarred)
                        {
                        privateMessage.IsStarred = false;
                        }
                    else
                        {
                        privateMessage.IsStarred = true;
                        }
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
                return null;
                }

            }      
        public void Delete(DeletePrivateMessageViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var message = _privateMessageService.Get(viewModel.Id);
                    if (message.UserTo.Id == LoggedOnReadOnlyUser.Id | message.UserFrom.Id == LoggedOnReadOnlyUser.Id)
                    {
                        _privateMessageService.DeleteMessage(message);
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
			}
        [HttpPost]
        public void DeleteAll(DeleteAllMessagesViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var sentMessages = _privateMessageService.GetAllSentToUser(LoggedOnReadOnlyUser.Id, viewModel.UserId).ToList();
                    foreach (var message in sentMessages)
                    {
                        _privateMessageService.DeleteMessage(message);
                    }
                    unitOfWork.SaveChanges();
                    var receivedMessages = _privateMessageService.GetAllReceivedFromUser(viewModel.UserId, LoggedOnReadOnlyUser.Id).ToList();                    
                        foreach (var message in receivedMessages)
                        {
                        _privateMessageService.DeleteMessage(message);
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
        [ChildActionOnly]
        public ActionResult Create(Guid to)
        {
            var settings = SettingsService.GetSettings();
            //  Thread.Sleep(1000);
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new CreatePrivateMessageViewModel
                {
                    To = to,
                    EnableAttachement = (bool?)settings.EnablePrivateMessagesAttachements ?? false
                };
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    // Check if member Blocked by receiver
                     if (loggedOnUser.BlockedByOtherUsers.Any(x => x.Blocker.Id == to && x.Blocked.Id == LoggedOnReadOnlyUser.Id))
                     {
                         return null;
                     } 
                    // Check if private message are enabled
                    if (!settings.EnablePrivateMessages || LoggedOnReadOnlyUser.DisablePrivateMessages == true)
                    {
                        return Content(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                    // Check outbox size of logged in user
                    var senderCount = _privateMessageService.GetAllSentByUser(LoggedOnReadOnlyUser.Id).Count;
                    if (senderCount > settings.MaxPrivateMessagesPerMember)
                    {
                        return Content(LocalizationService.GetResourceString("PM.SentItemsOverCapcity"));
                    }
                    if (senderCount > (settings.MaxPrivateMessagesPerMember - SiteConstants.Instance.PrivateMessageWarningAmountLessThanAllowedSize))
                    {
                        if (LoggedOnReadOnlyUser.DisableEmailNotifications != true)
                        {
                            // Send user a warning they are about to exceed 
                              var sb = new StringBuilder();
                              sb.Append($"<p>{LocalizationService.GetResourceString("PM.AboutToExceedInboxSizeBody")}</p>");
                              var email = new Email
                              {
                                  EmailTo = LoggedOnReadOnlyUser.Email,
                                  NameTo = LoggedOnReadOnlyUser.UserName,
                                  Subject = LocalizationService.GetResourceString("PM.AboutToExceedInboxSizeSubject")
                              };
                              email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                              _emailService.SendMail(email);
                        }
                        }
                    // Set editor permissions
                    ViewBag.ImageUploadType = permissions[SiteConstants.Instance.PermissionInsertEditorImages].IsTicked ? "forumimageinsert" : "image";
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                }
                return PartialView(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Create(CreatePrivateMessageViewModel viewModel)
        {
             if (Request.IsAjaxRequest())
            {
                var SendEmailNotification = false;
            var settings = SettingsService.GetSettings();
            var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
            var memberTo = MembershipService.GetUser(viewModel.To);
            if (!settings.EnablePrivateMessages || LoggedOnReadOnlyUser.DisablePrivateMessages == true)
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                        // Check the user they are trying to message hasn't blocked them
                       /* if (loggedOnUser.BlockedByOtherUsers.Any(x => x.Blocker.Id == memberTo.Id))
                        {
                            return Content(PmAjaxError(LocalizationService.GetResourceString("PM.BlockedMessage")));
                        }*/
                        // Check flood control
                        var lastMessage = _privateMessageService.GetLastSentPrivateMessage(LoggedOnReadOnlyUser.Id);
                        var lastSentMessage = _privateMessageService.GetLastMessageToUser(LoggedOnReadOnlyUser.Id, viewModel.To);
                        if (lastSentMessage == null || lastSentMessage.DateSent < DateTime.UtcNow.AddDays(-1))
                        {
                            SendEmailNotification = true;
                        }// If this message they are sending now, is to the same person then ignore flood control
                            if (lastMessage != null && viewModel.To == lastMessage.UserTo.Id)
                    {
                                if (DateUtils.TimeDifferenceInSeconds(DateTime.UtcNow, lastMessage.DateSent) < settings.PrivateMessageFloodControl)
                        {
                            return PartialView("_message", lastMessage);
                        }
                    }
                    if (memberTo != null)
                    {
                        // Map the view model to message
                        var privateMessage = new PrivateMessage
                        {
                            UserFrom = loggedOnUser,
                          //  UserTo = memberTo,
                            Message = viewModel.Message,
                        };
                        // Check settings
                        if (settings.EnableEmoticons == true)
                        {
                            privateMessage.Message = _configService.Emotify(privateMessage.Message);
                        }
                        // check they are not trying to message themself!
                        if (!string.Equals(memberTo.UserName, LoggedOnReadOnlyUser.UserName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Check in box size for both
                            var receiverCount = _privateMessageService.GetAllReceivedByUser(memberTo.Id).Count;
                            if (receiverCount > settings.MaxPrivateMessagesPerMember)
                            {
                                return Content(string.Format(LocalizationService.GetResourceString("PM.ReceivedItemsOverCapcity"), memberTo.UserName));
                            }
                            // If the receiver is about to go over the allowance them let then know too
                            if (receiverCount > (settings.MaxPrivateMessagesPerMember - SiteConstants.Instance.PrivateMessageWarningAmountLessThanAllowedSize))
                            {
                                // Send user a warning they are about to exceed 
                                var sb = new StringBuilder();
                                sb.Append($"<p>{LocalizationService.GetResourceString("PM.AboutToExceedInboxSizeBody")}</p>");
                                var email = new Email
                                {
                                    EmailTo = memberTo.Email,
                                    NameTo = memberTo.UserName,
                                    Subject = LocalizationService.GetResourceString("PM.AboutToExceedInboxSizeSubject")
                                };
                                email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                                _emailService.SendMail(email);
                            }

                            // Good to go send the message! 
                            privateMessage.UserTo = memberTo;
                            privateMessage = _privateMessageService.Add(privateMessage);
                              unitOfWork.SaveChanges();
                                //Attachements
                                if (Request.Files.Count > 0 && settings.EnablePrivateMessagesAttachements)
                            {
                                // store the file inside 
                                var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, loggedOnUser.Id));
                                if (!Directory.Exists(uploadFolderPath))
                                {
                                    Directory.CreateDirectory(uploadFolderPath);
                                }
                                //  Get all files from Request object  
                                HttpFileCollectionBase files = Request.Files;
                                for (int i = 0; i < files.Count; i++)
                                {
                                    HttpPostedFileBase file = files[i];
                                    var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService, false);
                                    if (!uploadResult.UploadSuccessful)
                                    {
                                        unitOfWork.Rollback();
                                    }
                                    // Add the filename to the database                            
                                    var attachment = new Attachment
                                    {
                                        Filename = uploadResult.UploadedFileName,
                                        PrivateMessage = privateMessage,
                                        MembershipUser = loggedOnUser,
                                        ContentType = file.ContentType
                                    };
                                    _attachmentService.Add(attachment);
                                    privateMessage.Attachments.Add(attachment);
                                }
                            }
                                try
                                {
                                    var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
                                      if (memberTo.LastActivityDate > date) {
                                        SendEmailNotification = false;
                                      }
                                                  // Finally send an email to the user so they know they have a new private message
                                                  // As long as they have not had notifications disabled
                                                    if (memberTo.DisableEmailNotifications != true && SendEmailNotification == true )
                                                      {
                                                          var email = new Email
                                                          {
                                                              EmailTo = memberTo.Email,
                                                              Subject = LocalizationService.GetResourceString("PM.NewPrivateMessageSubject"),
                                                              NameTo = memberTo.UserName
                                                          };

                                                          var sb = new StringBuilder();
                                                          sb.Append($"<p>{string.Format(LocalizationService.GetResourceString("PM.NewPrivateMessageBody"), LoggedOnReadOnlyUser.UserName)}</p>");
                                                          sb.Append(AppHelpers.ConvertPostContent(viewModel.Message));
                                                          email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                                                          _emailService.SendMail(email);
                                                      }
                                                                                      
                                    unitOfWork.Commit();
                                return PartialView("_message", privateMessage);
                            }
                            catch (Exception ex)
                            {
                                unitOfWork.Rollback();
                                LoggingService.Error(ex);
                                return Content(PmAjaxError(LocalizationService.GetResourceString("Errors.GenericMessage")));
                            }
                        }
                        else
                        {
                            return Content(PmAjaxError(LocalizationService.GetResourceString("PM.TalkToSelf")));
                        }
                    }
                    else
                    {
                        // Error send back to user
                        return Content(PmAjaxError(LocalizationService.GetResourceString("PM.UnableFindMember")));
                    }
                }
                return Content(PmAjaxError(LocalizationService.GetResourceString("Errors.GenericMessage")));
            }
        }
            else {
        return null;
        }
        }
        [Authorize]
        [ChildActionOnly]
        public ActionResult Notification()
        {
            var settings = SettingsService.GetSettings();
                var newMessages = _privateMessageService.GetNewMessages(LoggedOnReadOnlyUser.Id).DistinctBy(x => x.UserFrom).ToList();
            var canViewPms = settings.EnablePrivateMessages && LoggedOnReadOnlyUser != null && LoggedOnReadOnlyUser.DisablePrivateMessages != true;         
                var viewModel = new PrivateMessageNotificationViewModel
                {
                    NewPrivateMessageCount = canViewPms ? newMessages.Count : 0,
                    CanViewPrivateMessages = canViewPms,
                    NewMessages = newMessages
                };
                return PartialView(viewModel);
        }
    }
    }
