namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Security;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.Enums;
    using Core.Events;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Utilities;
    using Application;
    using Areas.Admin.ViewModels;
    using ViewModels;
    using ViewModels.Mapping;
    using Core.Domain.General;
    using Application.Akismet;
    using Core.Domain;

    public partial class TopicController : BaseController
    {
        const int AmountToShow = 5;
        private readonly IFavouriteService _favouriteService;
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly ITopicTagService _topicTagService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryNotificationService _categoryNotificationService;
        private readonly ITopicNotificationService _topicNotificationService;
        private readonly ITagNotificationService _tagNotificationService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly IEmailService _emailService;
        private readonly IPollService _pollService;
        private readonly IPollAnswerService _pollAnswerService;
        private readonly IBannedWordService _bannedWordService;
        private readonly IVoteService _voteService;
        private readonly IUploadedFileService _uploadedFileService;
        private readonly IPostEditService _postEditService;
        private readonly IActivityService _activityService;
        private readonly IReportService _reportService;
        public TopicController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IRoleService roleService, ITopicService topicService, IPostService postService, IActivityService activityService,
            ICategoryService categoryService, ILocalizationService localizationService, ISettingsService settingsService, ITopicTagService topicTagService, IMembershipUserPointsService membershipUserPointsService,
            ICategoryNotificationService categoryNotificationService, IEmailService emailService, ITopicNotificationService topicNotificationService, IPollService pollService,
            IPollAnswerService pollAnswerService, IBannedWordService bannedWordService, IVoteService voteService, IFavouriteService favouriteService, IUploadedFileService uploadedFileService, ICacheService cacheService,
            ITagNotificationService tagNotificationService, IPostEditService postEditService, IReportService reportService)
            : base(logAuditService,loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _topicService = topicService;
            _postService = postService;
            _categoryService = categoryService;
            _topicTagService = topicTagService;
            _membershipUserPointsService = membershipUserPointsService;
            _categoryNotificationService = categoryNotificationService;
            _emailService = emailService;
            _topicNotificationService = topicNotificationService;
            _pollService = pollService;
            _pollAnswerService = pollAnswerService;
            _bannedWordService = bannedWordService;
            _voteService = voteService;
            _favouriteService = favouriteService;
            _uploadedFileService = uploadedFileService;
            _tagNotificationService = tagNotificationService;
            _postEditService = postEditService;
            _activityService = activityService;
            _reportService = reportService;
        }
        [AllowAnonymous]
        public ActionResult Index(int? p)
        {          
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                // Set the page index
                var pageIndex = p ?? 1;
                var sortQuerystring = Request.QueryString[AppConstants.TopicViewOrder];
                var orderBy = !string.IsNullOrEmpty(sortQuerystring) ? EnumUtils.ReturnEnumValueFromString<TopicViewOrder>(sortQuerystring) : TopicViewOrder.Recent;
				// Get  topics
				var topics = _topicService.GetTopics(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.ActiveTopicsListSize, allowedCategories,orderBy);
                // Get Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, settings);
                // create the view model
                var viewModel = new ActiveTopicsViewModel
                {
                    Topics = topicViewModels,
					PageIndex = pageIndex,
					TotalCount = topics.TotalCount,
					TotalPages = topics.TotalPages
					};              
                return View(viewModel);
            }
        }
        [ChildActionOnly]
        [Authorize]
        public PartialViewResult TopicsMemberHasPostedIn(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                // Set the page index
                var pageIndex = p ?? 1;
				// Get the topics			
				var topics = _topicService.GetMembersActivity(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.MembersActivityListSize,LoggedOnReadOnlyUser.Id,allowedCategories);
                // Get the Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, settings);
                // create the view model
                var viewModel = new PostedInViewModel
                {
                    Topics = topicViewModels,
                    PageIndex = pageIndex,
                    TotalCount = topics.TotalCount,
                    TotalPages = topics.TotalPages
                };
                return PartialView(viewModel);
            }
        }
        [ChildActionOnly]
        [Authorize]
        public PartialViewResult GetSubscribedTopics()
        {
            var viewModel = new List<TopicViewModel>();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var topicIds = LoggedOnReadOnlyUser.TopicNotifications.Select(x => x.Topic.Id).ToList();
                if (topicIds.Any())
                {
                    var topics = _topicService.Get(topicIds, allowedCategories);

                    // Get the Topic View Models
                    viewModel = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());

                    // Show the unsubscribe link
                    foreach (var topicViewModel in viewModel)
                    {
                        topicViewModel.ShowUnSubscribedLink = true;
                    }
                }
            }
            return PartialView("GetSubscribedTopics", viewModel);
        }      
        [ChildActionOnly]
        public PartialViewResult GetTopicBreadcrumb(Topic topic)
        {
            var category = topic.Category;
            var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new BreadcrumbViewModel
                {
                    Categories = _categoryService.GetCategoryParents(category, allowedCategories),
                    Topic = topic
                };
                if (!viewModel.Categories.Any())
                {
                    viewModel.Categories.Add(topic.Category);
                }
                return PartialView("GetCategoryBreadcrumb", viewModel);
            }
        }
        public PartialViewResult CreateTopicButton()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new CreateTopicButtonViewModel
                {
                    LoggedOnUser = LoggedOnReadOnlyUser
                };
                if (LoggedOnReadOnlyUser.DisablePosting == true)
                {
                  //  var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                    var datefrom = DateTime.UtcNow.AddDays(-2);
                    var allreports = _reportService.GetReportsOnUser(LoggedOnReadOnlyUser, datefrom);
                    //To Enable Posting
                    if (allreports.Count() < SiteConstants.Instance.ReportCountToDisable)
                    {
                        LoggedOnReadOnlyUser.DisablePosting = false;
                        unitOfWork.SaveChanges();

                    }
                    else
                    {
                        viewModel.UserCanPostTopics = false;
                    }
                }

                if (LoggedOnReadOnlyUser != null && LoggedOnReadOnlyUser.DisablePosting != true)
                {
                    // Add all categories to a permission set
                    var allCategories = _categoryService.GetAll();

                    foreach (var category in allCategories)
                    {
                        // Now check to see if they have access to any categories
                        // if so, check they are allowed to create topics - If no to either set to false
                        viewModel.UserCanPostTopics = false;
                        var permissionSet = RoleService.GetPermissions(category, UsersRole);
                        if (permissionSet[SiteConstants.Instance.PermissionCreateTopics].IsTicked)
                        {
                            viewModel.UserCanPostTopics = true;
                            break;
                        }
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
                    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
                return PartialView(viewModel);
        }
        }
        [HttpPost]
        [Authorize]
        public JsonResult CheckTopicCreatePermissions(Guid catId)
        {
            if (Request.IsAjaxRequest())
            {
                var category = _categoryService.Get(catId);
                var permissionSet = RoleService.GetPermissions(category, UsersRole);
                var model = GetCheckCreateTopicPermissions(permissionSet);
                return Json(model);
            }
            return null;
        }
        #region Create / Edit Helper Methods
        private static CheckCreateTopicPermissions GetCheckCreateTopicPermissions(PermissionSet permissionSet)
        {
            var model = new CheckCreateTopicPermissions();

            if (permissionSet[SiteConstants.Instance.PermissionCreateStickyTopics].IsTicked)
            {
                model.CanStickyTopic = true;
            }

            if (permissionSet[SiteConstants.Instance.PermissionLockTopics].IsTicked)
            {
                model.CanLockTopic = true;
            }

            if (permissionSet[SiteConstants.Instance.PermissionAttachFiles].IsTicked)
            {
                model.CanUploadFiles = true;
            }

            if (permissionSet[SiteConstants.Instance.PermissionCreatePolls].IsTicked)
            {
                model.CanCreatePolls = true;
            }

            if (permissionSet[SiteConstants.Instance.PermissionInsertEditorImages].IsTicked)
            {
                model.CanInsertImages = true;
            }
            return model;
        }
        #endregion
        [Authorize]
        public ActionResult EditPostTopic(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Get the post
                var post = _postService.Get(id);

                // Get the topic
                var topic = post.Topic;

                // get the users permissions
                var permissions = RoleService.GetPermissions(topic.Category, UsersRole);

                // Is the user allowed to edit this post
                if (post.User.Id == LoggedOnReadOnlyUser.Id || permissions[SiteConstants.Instance.PermissionEditPosts].IsTicked)
                {
                    // Get the allowed categories for this user
                    var allowedAccessCategories = _categoryService.GetAllowedCategories(UsersRole);
                    var allowedCreateTopicCategories = _categoryService.GetAllowedCategories(UsersRole, SiteConstants.Instance.PermissionCreateTopics);
                    var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);

                    // If this user hasn't got any allowed cats OR they are not allowed to post then abandon
                    if (allowedAccessCategories.Any() && LoggedOnReadOnlyUser.DisablePosting != true)
                    {
                        // Create the model for just the post
                        var viewModel = new CreateEditTopicViewModel
                        {
                            Content = post.PostContent,
                            Id = post.Id,
                            Category = topic.Category.Id,
                            Name = topic.Name,
                            TopicId = topic.Id,
                            OptionalPermissions = GetCheckCreateTopicPermissions(permissions)
                        };

                        // Now check if this is a topic starter, if so add the rest of the field
                        if (post.IsTopicStarter)
                        {
                            // Remove all Categories that don't have create topic permission
                            allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));

                            // See if this user is subscribed to this topic
                            var topicNotifications = _topicNotificationService.GetByUserAndTopic(LoggedOnReadOnlyUser, topic);

                            // Populate the properties we can
                            viewModel.IsLocked = topic.IsLocked;
                            viewModel.IsSticky = topic.IsSticky;
                            viewModel.IsTopicStarter = post.IsTopicStarter;
                            viewModel.SubscribeToTopic = topicNotifications.Any();
                            viewModel.Categories = _categoryService.GetBaseSelectListCategories(allowedAccessCategories);

                            // Tags - Populate from the topic
                            if (topic.Tags.Any())
                            {
                                viewModel.Tags = string.Join<string>(",", topic.Tags.Select(x => x.Tag));
                            }

                            // Populate the poll answers
                            if (topic.Poll != null && topic.Poll.PollAnswers.Any())
                            {
                                // Has a poll so add it to the view model
                                viewModel.PollAnswers = topic.Poll.PollAnswers;
                                viewModel.PollCloseAfterDays = topic.Poll.ClosePollAfterDays ?? 0;
                            }
                        }

                        // Return the edit view
                        return View(viewModel);
                    }

                }

                // If we get here the user has no permission to try and edit the post
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPostTopic(CreateEditTopicViewModel editPostViewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Get the category
                var category = _categoryService.Get(editPostViewModel.Category);

                // First check this user is allowed to create topics in this category
                var permissions = RoleService.GetPermissions(category, UsersRole);

                // Now we have the category and permissionSet - Populate the optional permissions 
                // This is just in case the viewModel is return back to the view also sort the allowedCategories
                // Get the allowed categories for this user
                var allowedAccessCategories = _categoryService.GetAllowedCategories(UsersRole);
                var allowedCreateTopicCategories = _categoryService.GetAllowedCategories(UsersRole, SiteConstants.Instance.PermissionCreateTopics);
                var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);
                allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));
                editPostViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);
                editPostViewModel.Categories = _categoryService.GetBaseSelectListCategories(allowedAccessCategories);
                editPostViewModel.IsTopicStarter = editPostViewModel.Id == Guid.Empty;
                if (editPostViewModel.PollAnswers == null)
                {
                    editPostViewModel.PollAnswers = new List<PollAnswer>();
                }

                if (ModelState.IsValid)
                {

                    try
                    {
                        var topicPostInModeration = false;

                        // Check stop words
                        var stopWords = _bannedWordService.GetAll(true);
                        foreach (var stopWord in stopWords)
                        {
                            if (editPostViewModel.Content.IndexOf(stopWord.Word, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                                editPostViewModel.Name.IndexOf(stopWord.Word, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {

                                ShowMessage(new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("StopWord.Error"),
                                    MessageType = GenericMessages.danger
                                });

                                var p = _postService.Get(editPostViewModel.Id);
                                var t = p.Topic;

                                // Ahhh found a stop word. Abandon operation captain.
                                return Redirect(t.NiceUrl);
                            }
                        }

                        // Quick check to see if user is locked out, when logged in
                        if (LoggedOnReadOnlyUser.IsLockedOut || LoggedOnReadOnlyUser.DisablePosting == true || !LoggedOnReadOnlyUser.IsApproved)
                        {
                            FormsAuthentication.SignOut();
                            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoAccess"));
                        }

                        // Got to get a lot of things here as we have to check permissions
                        // Get the post
                        var post = _postService.Get(editPostViewModel.Id);

                        // Get the topic
                        var topic = post.Topic;

                        if (post.User.Id == LoggedOnReadOnlyUser.Id || permissions[SiteConstants.Instance.PermissionEditPosts].IsTicked)
                        {

                            // Get the DB user so we can use lazy loading and update
                            var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                            // Want the same edit date on both post and postedit
                            var dateEdited = DateTime.UtcNow;

                            // Create a post edit
                            var postEdit = new PostEdit
                            {
                                Post = post,
                                DateEdited = dateEdited,
                                EditedBy = loggedOnUser,
                                OriginalPostContent = post.PostContent,
                                OriginalPostTitle = post.IsTopicStarter ? topic.Name : string.Empty
                            };

                            // User has permission so update the post
                            post.PostContent = _bannedWordService.SanitiseBannedWords(editPostViewModel.Content);
                            post.DateEdited = dateEdited;

                            post = _postService.SanitizePost(post);

                            // Update postedit content
                            postEdit.EditedPostContent = post.PostContent;

                            // if topic starter update the topic
                            if (post.IsTopicStarter)
                            {
                                // if category has changed then update it
                                if (topic.Category.Id != editPostViewModel.Category)
                                {
                                    var cat = _categoryService.Get(editPostViewModel.Category);
                                    topic.Category = cat;
                                }
                                topic.IsLocked = editPostViewModel.IsLocked;
                                topic.IsSticky = editPostViewModel.IsSticky;
                                topic.Name = StringUtils.GetSafeHtml(_bannedWordService.SanitiseBannedWords(editPostViewModel.Name));

                                // Update post edit
                                postEdit.EditedPostTitle = topic.Name;

                                // See if there is a poll
                                if (editPostViewModel.PollAnswers != null && editPostViewModel.PollAnswers.Count(x => !string.IsNullOrEmpty(x?.Answer)) > 1 && permissions[SiteConstants.Instance.PermissionCreatePolls].IsTicked)
                                {
                                    // Now sort the poll answers, what to add and what to remove
                                    // Poll answers already in this poll.
                                    //var existingAnswers = topic.Poll.PollAnswers.Where(x => postedIds.Contains(x.Id)).ToList();
                                    var postedIds = editPostViewModel.PollAnswers.Where(x => !string.IsNullOrEmpty(x?.Answer)).Select(x => x.Id);

                                    // This post might not have a poll on it, if not they are creating a poll for the first time
                                    var topicPollAnswerIds = new List<Guid>();
                                    var pollAnswersToRemove = new List<PollAnswer>();
                                    if (topic.Poll == null)
                                    {
                                        // Create a new Poll
                                        var newPoll = new Poll
                                        {
                                            User = loggedOnUser
                                        };

                                        // Create the poll
                                        _pollService.Add(newPoll);

                                        // Save the poll in the context so we can add answers
                                        unitOfWork.SaveChanges();

                                        // Add the poll to the topic
                                        topic.Poll = newPoll;
                                    }
                                    else
                                    {
                                        topicPollAnswerIds = topic.Poll.PollAnswers.Select(p => p.Id).ToList();
                                        pollAnswersToRemove = topic.Poll.PollAnswers.Where(x => !postedIds.Contains(x.Id)).ToList();
                                    }

                                    // Set the amount of days to close the poll
                                    topic.Poll.ClosePollAfterDays = editPostViewModel.PollCloseAfterDays;

                                    var existingAnswers = editPostViewModel.PollAnswers.Where(x => !string.IsNullOrEmpty(x.Answer) && topicPollAnswerIds.Contains(x.Id)).ToList();
                                    var newPollAnswers = editPostViewModel.PollAnswers.Where(x => !string.IsNullOrEmpty(x.Answer) && !topicPollAnswerIds.Contains(x.Id)).ToList();

                                    // Loop through existing and update names if need be
                                    //TODO: Need to think about this in future versions if they change the name
                                    //TODO: As they could game the system by getting votes and changing name?
                                    foreach (var existPollAnswer in existingAnswers)
                                    {
                                        // Get the existing answer from the current topic
                                        var pa = topic.Poll.PollAnswers.FirstOrDefault(x => x.Id == existPollAnswer.Id);
                                        if (pa != null && pa.Answer != existPollAnswer.Answer)
                                        {
                                            // If the answer has changed then update it
                                            pa.Answer = existPollAnswer.Answer;
                                        }
                                    }

                                    // Loop through and remove the old poll answers and delete
                                    foreach (var oldPollAnswer in pollAnswersToRemove)
                                    {
                                        // Delete
                                        _pollAnswerService.Delete(oldPollAnswer);

                                        // Remove from Poll
                                        topic.Poll.PollAnswers.Remove(oldPollAnswer);
                                    }

                                    // Poll answers to add
                                    foreach (var newPollAnswer in newPollAnswers)
                                    {
                                        if (newPollAnswer != null)
                                        {
                                            var npa = new PollAnswer
                                            {
                                                Poll = topic.Poll,
                                                Answer = newPollAnswer.Answer
                                            };
                                            _pollAnswerService.Add(npa);
                                            topic.Poll.PollAnswers.Add(npa);
                                        }
                                    }
                                }
                                else
                                {
                                    // Need to check if this topic has a poll, because if it does
                                    // All the answers have now been removed so remove the poll.
                                    if (topic.Poll != null)
                                    {
                                        //Firstly remove the answers if there are any
                                        if (topic.Poll.PollAnswers != null && topic.Poll.PollAnswers.Any())
                                        {
                                            var answersToDelete = new List<PollAnswer>();
                                            answersToDelete.AddRange(topic.Poll.PollAnswers);
                                            foreach (var answer in answersToDelete)
                                            {
                                                // Delete
                                                _pollAnswerService.Delete(answer);

                                                // Remove from Poll
                                                topic.Poll.PollAnswers.Remove(answer);
                                            }
                                        }

                                        // Now delete the poll
                                        var pollToDelete = topic.Poll;
                                        _pollService.Delete(pollToDelete);

                                        // Remove from topic.
                                        topic.Poll = null;
                                    }
                                }

                                // Tags
                                topic.Tags.Clear();
                                if (!string.IsNullOrEmpty(editPostViewModel.Tags))
                                {
                                    _topicTagService.Add(editPostViewModel.Tags.ToLower(), topic);
                                }

                                // if the Category has moderation marked then the topic needs to 
                                // go back into moderation
                                if (topic.Category.ModerateTopics == true)
                                {
                                    topic.Pending = true;
                                    topicPostInModeration = true;
                                }

                                // Sort the post search field
                                post.SearchField = _postService.SortSearchField(post.IsTopicStarter, topic, topic.Tags);
                            }
                            else
                            {
                                // if the Category has moderation marked then the post needs to 
                                // go back into moderation
                                if (topic.Category.ModeratePosts == true)
                                {
                                    post.Pending = true;
                                    topicPostInModeration = true;
                                }
                            }

                            // Add the post edit too
                            _postEditService.Add(postEdit);

                            // Commit the changes
                            unitOfWork.Commit();

                            if (topicPostInModeration)
                            {
                                // If in moderation then let the user now
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
                                    MessageType = GenericMessages.info
                                };
                            }
                            else
                            {
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Post.Updated"),
                                    MessageType = GenericMessages.success
                                };
                            }

                            // redirect back to topic
                            return Redirect($"{topic.NiceUrl}?postbadges=true");
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericError"),
                            MessageType = GenericMessages.danger
                        };
                    }


                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                }
            }
            return View(editPostViewModel);
        }
        private CreateEditTopicViewModel PrePareCreateEditTopicViewModel(List<Category> allowedCategories)
        {
            var userIsAdmin = UserIsAdmin;
            var permissions = RoleService.GetPermissions(null, UsersRole);
            var canInsertImages = userIsAdmin;
            if (!canInsertImages)
            {
                canInsertImages = permissions[SiteConstants.Instance.PermissionInsertEditorImages].IsTicked;
            }
            return new CreateEditTopicViewModel
            {
                SubscribeToTopic = true,
                Categories = _categoryService.GetBaseSelectListCategories(allowedCategories),
                OptionalPermissions = new CheckCreateTopicPermissions
                {
                    CanLockTopic = userIsAdmin,
                    CanStickyTopic = userIsAdmin,
                    CanUploadFiles = userIsAdmin,
                    CanCreatePolls = userIsAdmin,
                    CanInsertImages = canInsertImages
                },
                PollAnswers = new List<PollAnswer>(),
                IsTopicStarter = true,
                PollCloseAfterDays = 0
            };
        }
        private List<Category> AllowedCreateCategories()
        {
            var allowedAccessCategories = _categoryService.GetAllowedCategories(UsersRole);
            var allowedCreateTopicCategories = _categoryService.GetAllowedCategories(UsersRole, SiteConstants.Instance.PermissionCreateTopics);
            var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);
            if (allowedAccessCategories.Any())
            {
                allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));
                allowedAccessCategories.RemoveAll(x => UsersRole.RoleName != AppConstants.AdminRoleName && x.IsLocked);
            }
            return allowedAccessCategories;
        }
        [Authorize]
        public ActionResult Create()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedAccessCategories = AllowedCreateCategories();

                if (allowedAccessCategories.Any() && LoggedOnReadOnlyUser.DisablePosting != true)
                {
                    var viewModel = PrePareCreateEditTopicViewModel(allowedAccessCategories);
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditTopicViewModel topicViewModel)
        {
            // Get the category
            var category = _categoryService.Get(topicViewModel.Category);

            // First check this user is allowed to create topics in this category
            var permissions = RoleService.GetPermissions(category, UsersRole);

            // Now we have the category and permissionSet - Populate the optional permissions 
            // This is just in case the viewModel is return back to the view also sort the allowedCategories
            topicViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);
            topicViewModel.Categories = _categoryService.GetBaseSelectListCategories(AllowedCreateCategories());
            topicViewModel.IsTopicStarter = true;
            if (topicViewModel.PollAnswers == null)
            {
                topicViewModel.PollAnswers = new List<PollAnswer>();
            }
            /*---- End Re-populate ViewModel ----*/

            if (ModelState.IsValid)
            {
                // Check posting flood control
                // Flood control test
                if (!_topicService.PassedTopicFloodTest(topicViewModel.Name, LoggedOnReadOnlyUser))
                {
                    // Failed test so don't post topic
                    return View(topicViewModel);
                }

                // Check stop words
                var stopWords = _bannedWordService.GetAll(true);
                foreach (var stopWord in stopWords)
                {
                    if (topicViewModel.Content.IndexOf(stopWord.Word, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        topicViewModel.Name.IndexOf(stopWord.Word, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("StopWord.Error"),
                            MessageType = GenericMessages.danger
                        });

                        // Ahhh found a stop word. Abandon operation captain.
                        return View(topicViewModel);
                    }
                }

                // Quick check to see if user is locked out, when logged in
                if (LoggedOnReadOnlyUser.IsLockedOut || LoggedOnReadOnlyUser.DisablePosting == true || !LoggedOnReadOnlyUser.IsApproved)
                {
                    FormsAuthentication.SignOut();
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoAccess"));
                }

                var successfullyCreated = false;
                var cancelledByEvent = false;
                var moderate = false;
                var topic = new Topic();

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    // Check this users role has permission to create a post
                    if (permissions[SiteConstants.Instance.PermissionDenyAccess].IsTicked || permissions[SiteConstants.Instance.PermissionReadOnly].IsTicked || !permissions[SiteConstants.Instance.PermissionCreateTopics].IsTicked)
                    {
                        // Add a model error that the user has no permissions
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.NoPermission"));
                    }
                    else
                    {
                        // We get the banned words here and pass them in, so its just one call
                        // instead of calling it several times and each call getting all the words back
                        var bannedWordsList = _bannedWordService.GetAll();
                        List<string> bannedWords = null;
                        if (bannedWordsList.Any())
                        {
                            bannedWords = bannedWordsList.Select(x => x.Word).ToList();
                        }

                        // Create the topic model
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        topic = new Topic
                        {
                            Name = _bannedWordService.SanitiseBannedWords(topicViewModel.Name, bannedWords),
                            Category = category,
                            User = loggedOnUser
                        };

                        // Check Permissions for topic topions
                        if (permissions[SiteConstants.Instance.PermissionLockTopics].IsTicked)
                        {
                            topic.IsLocked = topicViewModel.IsLocked;
                        }
                        if (permissions[SiteConstants.Instance.PermissionCreateStickyTopics].IsTicked)
                        {
                            topic.IsSticky = topicViewModel.IsSticky;
                        }

                        // See if the user has actually added some content to the topic
                        if (!string.IsNullOrEmpty(topicViewModel.Content))
                        {
                            // Check for any banned words
                            topicViewModel.Content = _bannedWordService.SanitiseBannedWords(topicViewModel.Content, bannedWords);

                            var e = new TopicMadeEventArgs { Topic = topic };
                            EventManager.Instance.FireBeforeTopicMade(this, e);
                            if (!e.Cancel)
                            {

                                // See if this is a poll and add it to the topic
                                if (topicViewModel.PollAnswers.Count(x => x != null) > 1)
                                {
                                    // Do they have permission to create a new poll
                                    if (permissions[SiteConstants.Instance.PermissionCreatePolls].IsTicked)
                                    {
                                        // Create a new Poll
                                        var newPoll = new Poll
                                        {
                                            User = loggedOnUser,
                                            ClosePollAfterDays = topicViewModel.PollCloseAfterDays
                                        };

                                        // Create the poll
                                        _pollService.Add(newPoll);

                                        // Save the poll in the context so we can add answers
                                        unitOfWork.SaveChanges();

                                        // Now sort the answers
                                        var newPollAnswers = new List<PollAnswer>();
                                        foreach (var pollAnswer in topicViewModel.PollAnswers)
                                        {
                                            if (pollAnswer.Answer != null)
                                            {
                                                // Attach newly created poll to each answer
                                                pollAnswer.Poll = newPoll;
                                                _pollAnswerService.Add(pollAnswer);
                                                newPollAnswers.Add(pollAnswer);
                                            }
                                        }
                                        // Attach answers to poll
                                        newPoll.PollAnswers = newPollAnswers;
                                        // Save the new answers in the context
                                        unitOfWork.SaveChanges();
                                        // Add the poll to the topic
                                        topic.Poll = newPoll;
                                    }
                                    else
                                    {
                                        //No permission to create a Poll so show a message but create the topic
                                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                        {
                                            Message = LocalizationService.GetResourceString("Errors.NoPermissionPolls"),
                                            MessageType = GenericMessages.info
                                        };
                                    }
                                }

                                // Check for moderation
                                if (category.ModerateTopics == true)
                                {
                                    topic.Pending = true;
                                    moderate = true;
                                }
                                // Create the topic
                                topic = _topicService.Add(topic);
                                // Save the changes
                                unitOfWork.SaveChanges();

                                // Now create and add the post to the topic
                                var topicPost = _topicService.AddLastPost(topic, topicViewModel.Content);

                                // Update the users points score for posting
                                _membershipUserPointsService.Add(new MembershipUserPoints
                                {
                                    Points = SettingsService.GetSettings().PointsAddedPerPost,
                                    User = loggedOnUser,
                                    PointsFor = PointsFor.Post,
                                    PointsForId = topicPost.Id
                                });
                                // Now check its not spam
                                var akismetHelper = new AkismetHelper(SettingsService);
                                if (akismetHelper.IsSpam(topic))
                                {
                                    topic.Pending = true;
                                    moderate = true;
                                }
                                if (topicViewModel.Files != null)
                                {
                                    // Get the permissions for this category, and check they are allowed to update
                                    if (permissions[SiteConstants.Instance.PermissionAttachFiles].IsTicked && LoggedOnReadOnlyUser.DisableFileUploads != true)
                                    {
                                        // woot! User has permission and all seems ok
                                        // Before we save anything, check the user already has an upload folder and if not create one
                                 var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, LoggedOnReadOnlyUser.Id));
                                        if (!Directory.Exists(uploadFolderPath))
                                        {
                                            Directory.CreateDirectory(uploadFolderPath);
                                        }
                                        // Loop through each file and get the file info and save to the users folder and Db
                                        foreach (var file in topicViewModel.Files)
                                        {
                                            if (file != null)
                                            {
                                                // If successful then upload the file
                                                var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService);

                                                if (!uploadResult.UploadSuccessful)
                                                {
                                                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                                    {
                                                        Message = uploadResult.ErrorMessage,
                                                        MessageType = GenericMessages.danger
                                                    };
                                                    unitOfWork.Rollback();
                                                    return View(topicViewModel);
                                                }

                                                // Add the filename to the database
                                                var uploadedFile = new UploadedFile
                                                {
                                                    Filename = uploadResult.UploadedFileName,
                                                    Post = topicPost,
                                                    MembershipUser = loggedOnUser
                                                };
                                                _uploadedFileService.Add(uploadedFile);
                                            }
                                        }
                                    }
                                }
                                // Add the tags if any too
                                if (!string.IsNullOrEmpty(topicViewModel.Tags))
                                {
                                    // Sanitise the tags
                                    topicViewModel.Tags = _bannedWordService.SanitiseBannedWords(topicViewModel.Tags,
                                        bannedWords);

                                    // Now add the tags
                                    _topicTagService.Add(topicViewModel.Tags.ToLower(), topic);
                                }

                                // After tags sort the search field for the post
                                topicPost.SearchField = _postService.SortSearchField(topicPost.IsTopicStarter, topic,topic.Tags);

                                //Cust
                              //  _notificationService.TopicCreated(topic);

                                // Subscribe the user to the topic as they have checked the checkbox
                                if (topicViewModel.SubscribeToTopic)
                                {
                                    // Create the notification
                                    var topicNotification = new TopicNotification
                                    {
                                        Topic = topic,
                                        User = loggedOnUser
                                    };
                                    //save
                                    _topicNotificationService.Add(topicNotification);
                                }
                            }
                            else
                            {
                                cancelledByEvent = true;
                            }
                            if (topic.Pending != true)
                            {
                                //Add To Member Topic Created Activity
                                _activityService.TopicCreated(topic);
                            }
                            try
                            {
                                unitOfWork.Commit();
                                if (!moderate)
                                {
                                    successfullyCreated = true;
                                }

                                // Only fire this if the create topic wasn't cancelled
                                if (!cancelledByEvent)
                                {
                                    EventManager.Instance.FireAfterTopicMade(this, new TopicMadeEventArgs { Topic = topic });
                                }
                            }
                            catch (Exception ex)
                            {
                                unitOfWork.Rollback();
                                LoggingService.Error(ex);
                                ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                            }

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                    }
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    if (successfullyCreated && !cancelledByEvent)
                    {
                        // Success so now send the emails
                        NotifyNewTopics(category, topic, unitOfWork);

                        //Add To Topic Created Activity
                        _activityService.TopicCreated(topic);
                        // Redirect to the newly created topic
                        return Redirect($"{topic.NiceUrl}?postbadges=true");
                    }
                    if (moderate)
                    {
                        // Moderation needed
                        // Tell the user the topic is awaiting moderation
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
                            MessageType = GenericMessages.info
                        };

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(topicViewModel);
        }
        public ActionResult Show(string slug, int? p)
        {
            // Set the page index
            var pageIndex = p ?? 1;
           
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Get the topic
                var topic = _topicService.GetTopicBySlug(slug);

                if (topic != null)
                {
                  
                    ////
                   ///To Add Member Recently Visited Topic
                    RecentActivity recentActivity = (RecentActivity)Session["RecentActivity"];
                    if (recentActivity == null)
                    {
                        recentActivity = new RecentActivity();
                        Session["RecentActivity"] = recentActivity;
                    }

                    /////// Method 1
                    recentActivity.AddRecentTopicVisited(topic);
                    ///////////Method 2
                   // GetRecentActivity().AddRecentTopicVisited(topic);

                    var settings = SettingsService.GetSettings();

                    // Note: Don't use topic.Posts as its not a very efficient SQL statement
                    // Use the post service to get them as it includes other used entities in one
                    // statement rather than loads of sql selects

                    var sortQuerystring = Request.QueryString[AppConstants.PostOrderBy];
                    var orderBy = !string.IsNullOrEmpty(sortQuerystring) ?
                                              EnumUtils.ReturnEnumValueFromString<PostOrderBy>(sortQuerystring) : PostOrderBy.Standard;

                    // Store the amount per page
                    var amountPerPage = settings.PostsPerPage;

                    if (sortQuerystring == AppConstants.AllPosts)
                    {
                        // Overide to show all posts
                        amountPerPage = int.MaxValue;
                    }

                    // Get the posts
                    var posts = _postService.GetPagedPostsByTopic(pageIndex, amountPerPage, int.MaxValue, topic.Id, orderBy);

                    // Get the topic starter post
                    var starterPost = _postService.GetTopicStarterPost(topic.Id);

                    // Get the permissions for the category that this topic is in
                    var permissions = RoleService.GetPermissions(topic.Category, UsersRole);

                    // If this user doesn't have access to this topic then
                    // redirect with message
                    if (permissions[SiteConstants.Instance.PermissionDenyAccess].IsTicked)
                    {
                        return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                    }

                    // Set editor permissions
                    ViewBag.ImageUploadType = permissions[SiteConstants.Instance.PermissionInsertEditorImages].IsTicked ? "forumimageinsert" : "image";

                    var viewModel = ViewModelMapping.CreateTopicViewModel(topic, permissions, posts.ToList(), starterPost, posts.PageIndex, posts.TotalCount, posts.TotalPages, LoggedOnReadOnlyUser, settings, true);
                    //  viewModel.IsSubscribed = UserIsAuthenticated && (_chatService.GetByUserAndRoom(LoggedOnReadOnlyUser, chatRoom).Any());
                   // viewModel.is
                    // If there is a quote querystring
                    var quote = Request["quote"];
                    if (!string.IsNullOrEmpty(quote))
                    {
                        try
                        {
                            // Got a quote
                            var postToQuote = _postService.Get(new Guid(quote));
                            viewModel.QuotedPost = postToQuote.PostContent;
                            viewModel.ReplyTo = postToQuote.Id;
                            viewModel.ReplyToUsername = postToQuote.User.UserName;
                        }
                        catch (Exception ex)
                        {
                            LoggingService.Error(ex);
                        }
                    }

                    var reply = Request["reply"];
                    if (!string.IsNullOrEmpty(reply))
                    {
                        try
                        {
                            // Set the reply
                            var toReply = _postService.Get(new Guid(reply));
                            viewModel.ReplyTo = toReply.Id;
                            viewModel.ReplyToUsername = toReply.User.UserName;
                        }
                        catch (Exception ex)
                        {
                            LoggingService.Error(ex);
                        }
                    }

                    var updateDatabase = false;

                    // User has permission lets update the topic view count
                    // but only if this topic doesn't belong to the user looking at it
                    var addView = !(UserIsAuthenticated && LoggedOnReadOnlyUser.Id == topic.User.Id);
                    if (addView)
                    {
                        updateDatabase = true;
                    }

                    // Check the poll - To see if it has one, and whether it needs to be closed.
                    if (viewModel.Poll?.Poll?.ClosePollAfterDays != null &&
                        viewModel.Poll.Poll.ClosePollAfterDays > 0 &&
                        !viewModel.Poll.Poll.IsClosed)
                    {
                        // Check the date the topic was created
                        var endDate = viewModel.Poll.Poll.DateCreated.AddDays((int)viewModel.Poll.Poll.ClosePollAfterDays);
                        if (DateTime.UtcNow > endDate)
                        {
                            topic.Poll.IsClosed = true;
                            viewModel.Topic.Poll.IsClosed = true;
                            updateDatabase = true;
                        }
                    }

                    if (!BotUtils.UserIsBot() && updateDatabase)
                    {
                        if (addView)
                        {
                            // Increase the topic views
                            topic.Views = (topic.Views + 1);
                        

                        // Update the users points
                        _membershipUserPointsService.Add(new MembershipUserPoints
                        {
                            Points = settings.PointsPerView,
                            User = topic.User,
                            PointsFor = PointsFor.View,
                            PointsForId = topic.Id
                        });
                    }
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            LoggingService.Error(ex);
                        }
                    }

                    return View(viewModel);
                }

            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
        [HttpPost]
        public PartialViewResult AjaxMorePosts(GetMorePostsViewModel getMorePostsViewModel)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Get the topic
                var topic = _topicService.Get(getMorePostsViewModel.TopicId);
                var settings = SettingsService.GetSettings();

                // Get the permissions for the category that this topic is in
                var permissions = RoleService.GetPermissions(topic.Category, UsersRole);

                // If this user doesn't have access to this topic then just return nothing
                if (permissions[SiteConstants.Instance.PermissionDenyAccess].IsTicked)
                {
                    return null;
                }

                var orderBy = !string.IsNullOrEmpty(getMorePostsViewModel.Order) ?
                                          EnumUtils.ReturnEnumValueFromString<PostOrderBy>(getMorePostsViewModel.Order) : PostOrderBy.Standard;

                var posts = _postService.GetPagedPostsByTopic(getMorePostsViewModel.PageIndex, settings.PostsPerPage, int.MaxValue, topic.Id, orderBy);
                var postIds = posts.Select(x => x.Id).ToList();
                var votes = _voteService.GetVotesByPosts(postIds);
                var favs = _favouriteService.GetAllPostFavourites(postIds);
                var viewModel = new ShowMorePostsViewModel
                {
                    Posts = ViewModelMapping.CreatePostViewModels(posts, votes, permissions, topic, LoggedOnReadOnlyUser, settings, favs),
                    Topic = topic,
                    Permissions = permissions
                };

                return PartialView(viewModel);
            }
        }
        public ActionResult TopicsByTag(string tag, int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                var tagModel = _topicTagService.Get(tag);

                // Set the page index
                var pageIndex = p ?? 1;

                // Get the topics
                var topics = _topicService.GetPagedTopicsByTag(pageIndex,
                                                           settings.TopicsPerPage,
                                                           int.MaxValue,
                                                           tag, allowedCategories);

                // See if the user has subscribed to this topic or not
                var isSubscribed = UserIsAuthenticated && (_tagNotificationService.GetByUserAndTag(LoggedOnReadOnlyUser, tagModel).Any());

                // Get the Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, settings);

                // create the view model
                var viewModel = new TagTopicsViewModel
                {
                    Topics = topicViewModels,
                    PageIndex = pageIndex,
                    TotalCount = topics.TotalCount,
                    TotalPages = topics.TotalPages,
                    Tag = tag,
                    IsSubscribed = isSubscribed,
                    TagId = tagModel.Id
                };

                return View(viewModel);
            }
        }    
        [ChildActionOnly]
        public ActionResult HotTopics(DateTime? from, DateTime? to, int? amountToShow)
        {            
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (amountToShow == null)
                {
                    amountToShow = 5;
                }
                var fromString = from != null ? Convert.ToDateTime(from).ToShortDateString() : null;
                var toString = to != null ? Convert.ToDateTime(to).ToShortDateString() : null;
                var cacheKey = string.Concat("HotTopics", UsersRole.Id, fromString, toString, amountToShow);
                var viewModel = CacheService.Get<HotTopicsViewModel>(cacheKey);
                
                if (viewModel == null)
                {
                    // Allowed Categories
                    var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                    // Get the topics
                    var topics = _topicService.GetPopularTopics(from, to, allowedCategories, (int)amountToShow);
                    // Get the Topic View Models
                    var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                    // create the view Model
                    viewModel = new HotTopicsViewModel
                    {
                        Topics = topicViewModels
                    };
                    CacheService.Set(cacheKey, viewModel, CacheTimes.TwoHours);
                }
                return PartialView(viewModel);
            }
        }
        private void NotifyNewTopics(Category cat, Topic topic, IUnitOfWork unitOfWork)
        {
            var settings = SettingsService.GetSettings();

            // Get all notifications for this category and for the tags on the topic
            var notifications = _categoryNotificationService.GetByCategory(cat).Select(x => x.User.Id).ToList();

            // Merge and remove duplicate ids
            if (topic.Tags != null && topic.Tags.Any())
            {
                var tagNotifications = _tagNotificationService.GetByTag(topic.Tags.ToList()).Select(x => x.User.Id).ToList();
                notifications = notifications.Union(tagNotifications).ToList();
            }

            if (notifications.Any())
            {
                // remove the current user from the notification, don't want to notify yourself that you 
                // have just made a topic!
                notifications.Remove(LoggedOnReadOnlyUser.Id);

                if (notifications.Count > 0)
                {
                    // Now get all the users that need notifying
                    var usersToNotify = MembershipService.GetUsersById(notifications);

                    // Create the email
                    var sb = new StringBuilder();
                    sb.AppendFormat("<p>{0}</p>", string.Format(LocalizationService.GetResourceString("Topic.Notification.NewTopics"), cat.Name));
                    sb.Append($"<p>{topic.Name}</p>");
                    if (SiteConstants.Instance.IncludeFullPostInEmailNotifications)
                    {
                        sb.Append(AppHelpers.ConvertPostContent(topic.LastPost.PostContent));
                    }
                    sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", string.Concat(Domain, cat.NiceUrl));

                    // create the emails and only send them to people who have not had notifications disabled
                    var emails = usersToNotify.Where(x => x.DisableEmailNotifications != true && !x.IsLockedOut && x.IsBanned != true).Select(user => new Email
                    {
                        Body = _emailService.EmailTemplate(user.UserName, sb.ToString()),
                        EmailTo = user.Email,
                        NameTo = user.UserName,
                        Subject = string.Concat(LocalizationService.GetResourceString("Topic.Notification.Subject"), settings.ForumName)
                    }).ToList();

                    // and now pass the emails in to be sent
                    _emailService.SendMail(emails);

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
            }
        }   
        public ActionResult TodayTopics(int? amountToShow)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (amountToShow == null)
                {
                    amountToShow = SiteConstants.Instance.TopicsAsideListSize;
                }
                //// Allowed Categories
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var topics = _topicService.GetTodaysTopics(AmountToShow, allowedCategories).ToList();           
                // Get the Topic View Models
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                var  viewModel = new ActiveTopicsViewModel
                {
                    Topics = topicViewModels
                };
            return PartialView(viewModel);
        }
    }
        [HttpPost]
        public PartialViewResult GetSimilarTopics(string searchTerm)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Returns the formatted string to search on
                var formattedSearchTerm = StringUtils.ReturnSearchString(searchTerm);
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                IList<Topic> topics = null;
                try
                {
                    var searchResults = _topicService.SearchTopics(SiteConstants.Instance.SimilarTopicsListSize, formattedSearchTerm, allowedCategories);
                    if (searchResults != null)
                    {
                        topics = searchResults;
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                }

                // Pass the list to the partial view
                return PartialView(topics);
            }
        } 
        public virtual PartialViewResult NewsAndEvent()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();

                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var category = _categoryService.GetNewsCategory();
               // var section = category.Section;
                var topics = _topicService.GetNewsandEvent(5, category.Id);
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics.ToList(), RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                // Create the main view model for the news
                var viewModel = new ActiveTopicsViewModel
                {
                    Topics = topicViewModels,
                    Category = category
                };
                return PartialView(viewModel);
            }
        }
        public ActionResult RelatedTopics(Guid topicId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Get topic
                var topic = _topicService.Get(topicId);
                // Get category
                var category = topic.Category;
                //Get all topics
                var topics = _topicService.GetAllTopicsByCategory(category.Id, SiteConstants.Instance.TopicsAsideListSize).Distinct().Take(6).OrderByDescending(x => x.CreateDate).ToList();
				//Related topics
				/* var relatedTopics = topics.Where(x => x.User.Id != LoggedOnReadOnlyUser.Id && x.Id != topic.Id).ToList();*/
				var relatedTopics = topics.Where(x => x.Id != topic.Id).ToList();
				var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);   
                var viewModel = new ActiveTopicsViewModel();
                try
                {                  
                    if (relatedTopics != null)
                    {
                        // Get the Topic View Models
                        var topicViewModels = ViewModelMapping.CreateTopicViewModels(relatedTopics.ToList(), RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                        // create the view model
                        viewModel = new ActiveTopicsViewModel
                        {
                            Topics = topicViewModels,
                           Category = category
                        };
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                }
                // Pass the list to the partial view
                return PartialView(viewModel);
            }
        }
        [Authorize]
        public ActionResult Share(Guid topicId)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var topic = _topicService.Get(topicId);
                    var starterPost = _postService.GetTopicStarterPost(topic.Id);
                    var viewModel = new ShareTopicViewModel
                    {
                        Topic = topic,
                          StartPost = starterPost
                    };
                    return PartialView(viewModel);
                }
            }
            return null;
        }
        [Authorize]
        public ActionResult Invitation(Guid topicId)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var topic = _topicService.Get(topicId);
                    var starterPost = _postService.GetTopicStarterPost(topic.Id);
                    var viewModel = new TopicInvitationViewModel
                    {
                        TopicId = topicId,
                        Topic = topic,
                        StartPost = starterPost
                };
                    // Add empty strings for initial input boxes if desired
                    // viewModel.Emails.AddRange(new[] { "", "", "" });
                    var Guest = new InvitationToViewModel {
                        Name = "",
                        Email = ""
                    };
                    viewModel.InvitationTo.Add(Guest);
                    return PartialView(viewModel);
                }
            }
            return null;
        }
        [HttpPost]
        [Authorize]
       // [ValidateAntiForgeryToken]
        public ActionResult Invitation(TopicInvitationViewModel viewModel)
        {
           // if (ModelState.IsValid)
           // {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var settings = SettingsService.GetSettings();
                var successfullySent = false;
                // Get topic
                var topic = _topicService.Get(viewModel.TopicId);
                    // Get topic starter post
                    var starterPost = _postService.GetTopicStarterPost(topic.Id);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    if (viewModel.InvitationTo != null && viewModel.InvitationTo.Any())
                    {
                    try
                    {
                        var sb = new StringBuilder();
                        /* sb.AppendFormat("<p>{2} <a href=\"{0}\">{1}</a></p>", string.Concat(settings.ForumUrl.TrimEnd('/'), loggedOnUser.NiceUrl), loggedOnUser.UserName, "");*/
                        /*  sb.AppendFormat("<p>{2} : <a href=\"{0}\">{1}</a></p>", string.Concat(settings.ForumUrl.TrimEnd('/'), loggedOnUser.NiceUrl), string.Concat(settings.ForumUrl.TrimEnd('/'), topic.NiceUrl), topic.Name, LocalizationService.GetResourceString("Invites discuusions in topic"));*/
                        sb.AppendFormat("<p><a href=\"{0}\">{1}</a>  {4}  <a href=\"{2}\">{3}</a></p>", string.Concat(settings.ForumUrl.TrimEnd('/'), loggedOnUser.NiceUrl), loggedOnUser.UserName, string.Concat(settings.ForumUrl.TrimEnd('/'), topic.NiceUrl), topic.Name, LocalizationService.GetResourceString("Topic.Invitation.Invites"));
                        sb.Append(string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(AppHelpers.ConvertPostContent(starterPost.PostContent)), 25), "..."));
                        // sb.Append(AppHelpers.ConvertPostContent(starterPost.PostContent));                    
                        foreach (var invitation in viewModel.InvitationTo.DistinctBy(x => x.Email))
                        {
                            if (invitation.Name != null && invitation.Email != null)
                            {
                                // Create the email
                                var email = new Email
                                {
                                    EmailTo = invitation.Email,
                                    NameTo = invitation.Name,
                                    Subject = string.Concat(LocalizationService.GetResourceString("Topic Invitation in" + " "), settings.ForumName),
                                    Body = _emailService.EmailTemplate(invitation.Name, sb.ToString())
                                };
                                _emailService.SendMail(email);
                                successfullySent = true;
                            }
                        }
                        if (successfullySent) {
                        // Add points to users for invitation
                        _membershipUserPointsService.Add(new MembershipUserPoints
                        {
                            Points = SiteConstants.Instance.PointsForInvitation,
                            User = loggedOnUser,
                            PointsFor = PointsFor.Invitation,
                            PointsForId = loggedOnUser.Id
                        });
                    }
                        unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Topic.Invitation.Sent"),
                                MessageType = GenericMessages.success
                            };
                        
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Topic.Invitation.Failed"),
                            MessageType = GenericMessages.info
                        });
                    }
                }
                return Redirect($"{topic.NiceUrl}");
            }
        }
        [HttpPost]
        public PartialViewResult GetMemberDiscussions(int? p, Guid id)
        {
            if (Request.IsAjaxRequest())
            {
                var pageIndex = p ?? 1;
               // var amountPerPage = int.MaxValue;
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                   // var topics = _topicService.GetMembersDiscussion(pageIndex, SiteConstants.Instance.PagingGroupSize, amountPerPage,id, allowedCategories);
                    var topics = _topicService.GetMembersDiscussion(pageIndex, SiteConstants.Instance.PagingGroupSize, SiteConstants.Instance.MembersActivityListSize, id, allowedCategories);
                    var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                    // create the view model
                    var viewModel = new MemberDiscussionsViewModel
                    {
                        Topics = topicViewModels,
                        PageIndex = pageIndex,
                        TotalCount = topics.TotalCount,
                        TotalPages = topics.TotalPages,
                        UserId = id
                    };
                    return PartialView(viewModel);
                }
            }
            return null;
        }
        [HttpPost]
        public PartialViewResult AjaxMoreMemberDiscussions(GetMoreDiscussionsViewModel getMoreDiscussionsViewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                    var settings = SettingsService.GetSettings();
                    var topics = _topicService.GetMembersDiscussion(getMoreDiscussionsViewModel.PageIndex, SiteConstants.Instance.PagingGroupSize, SiteConstants.Instance.MembersActivityListSize, getMoreDiscussionsViewModel.UserId, allowedCategories);
                    var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, SettingsService.GetSettings());
                    // create the view model
                    var viewModel = new MemberDiscussionsViewModel
                    {
                        Topics = topicViewModels,
                        PageIndex = getMoreDiscussionsViewModel.PageIndex,
                        TotalCount = topics.TotalCount,
                        TotalPages = topics.TotalPages,
                        UserId = getMoreDiscussionsViewModel.UserId
                    };
                    return PartialView(viewModel);
                }
            }
            return null;
        }
        [Authorize]
        [ChildActionOnly]
        public ActionResult Notification()
        {
            var newnotificationCount = 0;
            var moderateCount = 0;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);              
              //  var notificationlastcheck = loggedOnUser.NotificationLastChecked;
                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole).ToList();
                moderateCount += _topicService.GetPendingTopics(allowedCategories, UsersRole, LoggedOnReadOnlyUser.Id).Count;
                moderateCount += _postService.GetPendingPosts(allowedCategories, UsersRole, LoggedOnReadOnlyUser.Id).Count;
                //To Get New Category Notification
                var categoryIds = loggedOnUser.CategoryNotifications.Select(x => x.Category.Id).ToList();
                // Get the distinct topics             
                var topics = _topicService.GetTopicNotifications(categoryIds, loggedOnUser).Distinct().ToList();
                newnotificationCount += topics.Count(x => x.CreateDate > loggedOnUser.NotificationLastChecked);
                //To Get New Topic Notification
                var topicIds = loggedOnUser.TopicNotifications.Select(x => x.Topic.Id).ToList();
                var posts = _postService.GetPostNotifications(topicIds, loggedOnUser).Distinct().ToList();
                newnotificationCount += posts.Count(x => x.DateCreated > loggedOnUser.NotificationLastChecked);
                // create the view model
                var viewModel = new NotificationsViewModel
                {
                    Topics = topics.Take(5).ToList(),
                    Posts = posts.Take(5).ToList(),
                    NewNotification = newnotificationCount,
                    ModerateCount = moderateCount,
                    LoggedOnUser = loggedOnUser
                };
                /*if (Request.IsAjaxRequest())
                {
                loggedOnUser.NotificationLastChecked = DateTime.UtcNow;
                    return null;
                }*/
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
                return PartialView(viewModel);
            }
        }
        [Authorize]
        [ChildActionOnly]
        public ActionResult Suggestion()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                DateTime from = DateTime.UtcNow.AddDays(-14);

                var allowedCategories = _categoryService.GetAllowedCategories(UsersRole);
                var settings = SettingsService.GetSettings();
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                var notificationlastcheck = loggedOnUser.NotificationLastChecked;

                //To Get New Category Topics Suggestion
                var categoryIds = loggedOnUser.CategoryNotifications.Select(x => x.Category.Id).ToList();
                var topicIds = loggedOnUser.TopicNotifications.Select(x => x.Topic.Category.Id).ToList();
                var ids = categoryIds.Concat(topicIds).ToList();
                // var topics = _topicService.GetTopicNotifications(categoryIds, loggedOnUser).Distinct().Take(6).OrderByDescending(x => x.CreateDate).ToList();
                // Get the distinct topics 
                var suggestionTopics = _topicService.GetSuggestionTopics(from, ids, loggedOnUser, SiteConstants.Instance.TopicsAsideListSize).Distinct().ToList();

                // create the view model
                var topicViewModels = ViewModelMapping.CreateTopicViewModels(suggestionTopics, RoleService, UsersRole, LoggedOnReadOnlyUser, allowedCategories, settings);

                // create the view model
                var viewModel = new ActiveTopicsViewModel
                {
                    Topics = topicViewModels,
                };
                return PartialView(viewModel);
            }
        }    
    }
        }