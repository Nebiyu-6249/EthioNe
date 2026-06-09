using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.WebUI.Application;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EthioNegarit.WebUI.ViewModels.Mapping
{
    public static class ViewModelMapping
    {
        #region Membership

        public static SingleMemberListViewModel UserToSingleMemberListViewModel(MembershipUser user)
        {
            var viewModel = new SingleMemberListViewModel
            {
                IsApproved = user.IsApproved,
                Id = user.Id,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserName = user.UserName
            };
            return viewModel;
        }

        public static MemberEditViewModel UserToMemberEditViewModel(MembershipUser user)
        {
            var viewModel = new MemberEditViewModel
            {
                IsApproved = user.IsApproved,
                Id = user.Id,
                IsLockedOut = user.IsLockedOut,
                IsBanned = user.IsBanned,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserName = user.UserName,
                Age = user.Age,
                Comment = user.Comment,
                Email = user.Email,
                Facebook = user.Facebook,
                Location = user.Location,
                PasswordAnswer = user.PasswordAnswer,
                PasswordQuestion = user.PasswordQuestion,
                Signature = user.Signature,
                Twitter = user.Twitter,
                Website = user.Website,
                DisableEmailNotifications = (user.DisableEmailNotifications == true),
                DisablePosting = (user.DisablePosting == true),
                DisablePrivateMessages = (user.DisablePrivateMessages == true),
                DisableFileUploads = (user.DisableFileUploads == true),
                Avatar = user.Avatar
            };
            return viewModel;
        }

        public static RoleViewModel RoleToRoleViewModel(MembershipRole role)
        {
            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
            return viewModel;
        }

        public static MembershipRole RoleViewModelToRole(RoleViewModel roleViewModel)
        {
            var viewModel = new MembershipRole
            {
                RoleName = roleViewModel.RoleName
            };
            return viewModel;
        }

        #endregion

        #region Settings
        public static Settings SettingsViewModelToSettings(EditSettingsViewModel settingsViewModel, Settings existingSettings)
        {
            //NOTE: The only reason some properties are commented out, are because those items were
            //      moved to their own page when the admin was refactored.

            existingSettings.Id = settingsViewModel.Id;
            existingSettings.ForumName = settingsViewModel.ForumName;
            existingSettings.ForumUrl = settingsViewModel.ForumUrl;
            existingSettings.IsClosed = settingsViewModel.IsClosed;
            existingSettings.EnableRSSFeeds = settingsViewModel.EnableRSSFeeds;
            existingSettings.DisplayEditedBy = settingsViewModel.DisplayEditedBy;
            existingSettings.EnableMarkAsSolution = settingsViewModel.EnableMarkAsSolution;
            existingSettings.MarkAsSolutionReminderTimeFrame = settingsViewModel.MarkAsSolutionReminderTimeFrame;
            //existingSettings.EnableSpamReporting = settingsViewModel.EnableSpamReporting;
            //existingSettings.EnableMemberReporting = settingsViewModel.EnableMemberReporting;
            existingSettings.EnableEmailSubscriptions = settingsViewModel.EnableEmailSubscriptions;
            existingSettings.ManuallyAuthoriseNewMembers = settingsViewModel.ManuallyAuthoriseNewMembers;
            existingSettings.EmailAdminOnNewMemberSignUp = settingsViewModel.EmailAdminOnNewMemberSignUp;
            existingSettings.TopicsPerPage = settingsViewModel.TopicsPerPage;
            existingSettings.PostsPerPage = settingsViewModel.PostsPerPage;
            existingSettings.ActivitiesPerPage = settingsViewModel.ActivitiesPerPage;
            existingSettings.EnablePrivateMessages = settingsViewModel.EnablePrivateMessages;
            existingSettings.EnablePrivateMessagesAttachements = settingsViewModel.EnablePrivateMessagesAttachements;            
            existingSettings.MaxPrivateMessagesPerMember = settingsViewModel.MaxPrivateMessagesPerMember;
            existingSettings.PrivateMessageFloodControl = settingsViewModel.PrivateMessageFloodControl;
            existingSettings.EnableSignatures = settingsViewModel.EnableSignatures;
            existingSettings.EnablePoints = settingsViewModel.EnablePoints;
            existingSettings.PointsAllowedToVoteAmount = settingsViewModel.PointsAllowedToVoteAmount;
            existingSettings.PointsAllowedForExtendedProfile = settingsViewModel.PointsAllowedForExtendedProfile;
            existingSettings.PointsAddedPerPost = settingsViewModel.PointsAddedPerPost;
            existingSettings.PointsAddedPostiveVote = settingsViewModel.PointsAddedPostiveVote;
            existingSettings.PointsDeductedNagativeVote = settingsViewModel.PointsDeductedNagativeVote;
            existingSettings.PointsAddedForSolution = settingsViewModel.PointsAddedForSolution;
            existingSettings.AdminEmailAddress = settingsViewModel.AdminEmailAddress;
            existingSettings.NotificationReplyEmail = settingsViewModel.NotificationReplyEmail;
            existingSettings.SMTP = settingsViewModel.SMTP;
            existingSettings.SMTPUsername = settingsViewModel.SMTPUsername;
            existingSettings.SMTPPassword = settingsViewModel.SMTPPassword;
            existingSettings.Theme = settingsViewModel.Theme;
            //existingSettings.AkismentKey = settingsViewModel.AkismentKey;
            //existingSettings.EnableAkisment = settingsViewModel.EnableAkisment;
            existingSettings.SMTPPort = settingsViewModel.SMTPPort.ToString();
            //existingSettings.SpamQuestion = settingsViewModel.SpamQuestion;
            //existingSettings.SpamAnswer = settingsViewModel.SpamAnswer;
            existingSettings.SMTPEnableSSL = settingsViewModel.SMTPEnableSSL;
            //existingSettings.EnableSocialLogins = settingsViewModel.EnableSocialLogins;
            existingSettings.EnablePolls = settingsViewModel.EnablePolls;
            existingSettings.SuspendRegistration = settingsViewModel.SuspendRegistration;
            existingSettings.NewMemberEmailConfirmation = settingsViewModel.NewMemberEmailConfirmation;
            existingSettings.PageTitle = settingsViewModel.PageTitle;
            existingSettings.MetaDesc = settingsViewModel.MetaDesc;
            existingSettings.EnableEmoticons = settingsViewModel.EnableEmoticons;
            existingSettings.DisableDislikeButton = settingsViewModel.DisableDislikeButton;
            existingSettings.AgreeToTermsAndConditions = settingsViewModel.AgreeToTermsAndConditions;
            existingSettings.DisableStandardRegistration = settingsViewModel.DisableStandardRegistration;
            /////////////////////////////////////////////////          
            existingSettings.AdCostPerMonth = settingsViewModel.AdCostPerMonth;        
            existingSettings.PointsPerView = settingsViewModel.PointsPerView;
          //  existingSettings.AccountTempCloseDays = settingsViewModel.AccountTempCloseDays;        
            existingSettings.EnableAd = settingsViewModel.EnableAd;
            existingSettings.EnableChat = settingsViewModel.EnableChat;
           // existingSettings.TermsAndConditions = settingsViewModel.TermsAndConditions;

            existingSettings.Address = settingsViewModel.Address;
            existingSettings.YearStarted = settingsViewModel.YearStarted;
          existingSettings.PointsDeductPerSpam = settingsViewModel.PointsDeductPerSpam;
          existingSettings.SpamCountToBan = settingsViewModel.SpamCountToBan;
        existingSettings.ReportCountsToFlaggedAsSpam = settingsViewModel.ReportCountsToFlaggedAsSpam;
            existingSettings.TempLockedDays = settingsViewModel.TempLockedDays;
            existingSettings.ReportCountsToLock = settingsViewModel.ReportCountsToLock;
            existingSettings.AdTrialDays = settingsViewModel.AdTrialDays;
            /*
                        existingSettings.TermsAndConditionsAm = settingsViewModel.TermsAndConditionsAm;
            existingSettings.PrivacyPolicy = settingsViewModel.PrivacyPolicy;
            existingSettings.PrivacyPolicyAm = settingsViewModel.PrivacyPolicyAm;
            */
            //  existingSettings.DefaultListPerPage = settingsViewModel.DefaultListPerPage;
            return existingSettings;
        }

        public static EditSettingsViewModel SettingsToSettingsViewModel(Settings currentSettings)
        {
            var settingViewModel = new EditSettingsViewModel
            {
                Id = currentSettings.Id,
                ForumName = currentSettings.ForumName,
                ForumUrl = currentSettings.ForumUrl,
                IsClosed = currentSettings.IsClosed,
                EnableRSSFeeds = currentSettings.EnableRSSFeeds,
                DisplayEditedBy = currentSettings.DisplayEditedBy,
                EnableMarkAsSolution = currentSettings.EnableMarkAsSolution,
                MarkAsSolutionReminderTimeFrame = currentSettings.MarkAsSolutionReminderTimeFrame ?? 0,
                //EnableSpamReporting = currentSettings.EnableSpamReporting,
                //EnableMemberReporting = currentSettings.EnableMemberReporting,
                EnableEmailSubscriptions = currentSettings.EnableEmailSubscriptions,
                ManuallyAuthoriseNewMembers = currentSettings.ManuallyAuthoriseNewMembers,
                EmailAdminOnNewMemberSignUp = currentSettings.EmailAdminOnNewMemberSignUp,
                TopicsPerPage = currentSettings.TopicsPerPage,
                PostsPerPage = currentSettings.PostsPerPage,
                ActivitiesPerPage = currentSettings.ActivitiesPerPage,
                EnablePrivateMessages = currentSettings.EnablePrivateMessages,
                MaxPrivateMessagesPerMember = currentSettings.MaxPrivateMessagesPerMember,
                PrivateMessageFloodControl = currentSettings.PrivateMessageFloodControl,
                EnableSignatures = currentSettings.EnableSignatures,
                EnablePoints = currentSettings.EnablePoints,
                PointsAllowedToVoteAmount = currentSettings.PointsAllowedToVoteAmount,
                PointsAllowedForExtendedProfile = currentSettings.PointsAllowedForExtendedProfile ?? 0,
                PointsAddedPerPost = currentSettings.PointsAddedPerPost,
                PointsAddedPostiveVote = currentSettings.PointsAddedPostiveVote,
                PointsDeductedNagativeVote = currentSettings.PointsDeductedNagativeVote,
                PointsAddedForSolution = currentSettings.PointsAddedForSolution,
                AdminEmailAddress = currentSettings.AdminEmailAddress,
                NotificationReplyEmail = currentSettings.NotificationReplyEmail,
                SMTP = currentSettings.SMTP,
                SMTPUsername = currentSettings.SMTPUsername,
                SMTPPassword = currentSettings.SMTPPassword,
                //AkismentKey = currentSettings.AkismentKey,
                //EnableAkisment = currentSettings.EnableAkisment != null && (bool)currentSettings.EnableAkisment,
                NewMemberEmailConfirmation = currentSettings.NewMemberEmailConfirmation != null && (bool)currentSettings.NewMemberEmailConfirmation,
                Theme = currentSettings.Theme,
                SMTPPort = string.IsNullOrEmpty(currentSettings.SMTPPort) ? null : (int?)(Convert.ToInt32(currentSettings.SMTPPort)),
                //SpamQuestion = currentSettings.SpamQuestion,
                //SpamAnswer = currentSettings.SpamAnswer,
                Themes = AppHelpers.GetThemeFolders(),
                SMTPEnableSSL = currentSettings.SMTPEnableSSL ?? false,
                //EnableSocialLogins = currentSettings.EnableSocialLogins ?? false,
                EnablePolls = currentSettings.EnablePolls ?? false,
                SuspendRegistration = currentSettings.SuspendRegistration ?? false,
                PageTitle = currentSettings.PageTitle,
                MetaDesc = currentSettings.MetaDesc,
                EnableEmoticons = currentSettings.EnableEmoticons == true,
                DisableDislikeButton = currentSettings.DisableDislikeButton,
                AgreeToTermsAndConditions = currentSettings.AgreeToTermsAndConditions ?? false,
                DisableStandardRegistration = currentSettings.DisableStandardRegistration ?? false,
            AdCostPerMonth = currentSettings.AdCostPerMonth ?? 1000,          
            PointsPerView = (int?)currentSettings.PointsPerView ?? 1,
          //  AccountTempCloseDays = (int ?)currentSettings.AccountTempCloseDays ?? 30,       
            EnableAd = (bool?)currentSettings.EnableAd ?? false,
            EnableChat = (bool?)currentSettings.EnableChat ?? false,
          //  EnableJobandVaccancy = (bool?)currentSettings.EnableJobandVaccancy ?? false,
                Address = currentSettings.Address,
                YearStarted = currentSettings.YearStarted,
            PointsDeductPerSpam = (int?)currentSettings.PointsDeductPerSpam ?? 1,
                TempLockedDays = (int?)currentSettings.TempLockedDays ?? 7,
             ReportCountsToFlaggedAsSpam = (int ?)currentSettings.ReportCountsToFlaggedAsSpam ?? 1,
                SpamCountToBan = (int?)currentSettings.SpamCountToBan ?? 1,
                ReportCountsToLock = (int?)currentSettings.ReportCountsToLock ?? 1,
                AdTrialDays = (int?)currentSettings.AdTrialDays ?? 0,
               EnablePrivateMessagesAttachements = (bool?)currentSettings.EnablePrivateMessagesAttachements ?? true
        };

            return settingViewModel;
        }
        #endregion

        #region Topics
        public static Dictionary<Category, PermissionSet> GetPermissionsForTopics(IEnumerable<Topic> topics, IRoleService roleService, MembershipRole usersRole)
        {
            // Get all the categories for this topic collection
            var categories = topics.Select(x => x.Category).Distinct();

            return GetPermissionsForCategories(categories, roleService, usersRole);
        }

        public static List<ReportViewModel> CreateMessageReportViewModels(List<Report> reports)
        {

            var topicIds = reports.Select(x => x.Id).ToList();

            // Gets posts for topics
            var postService = ServiceFactory.Get<IPostService>();
            // var posts = postService.GetPostsByTopics(topicIds, allowedCategories);
            // var groupedPosts = posts.ToLookup(x => x.Topic.Id);
            var viewModels = new List<ReportViewModel>();
            return viewModels;
        }
        public static List<ReportViewModel> CreatePostReportViewModels(List<Report> reports) {

         //   var postIds = reports.Select(x => x.ReferenceId).ToList();
          //  var reportersIds = reports.Select(x => x.Reporter.Id).ToList();
            var reportIds = reports.Select(x => x.Id).ToList();
            // Gets Reporters for Reports
            var membershipService = ServiceFactory.Get<IMembershipService>();
//var reporters = membershipService.GetMembers(reportersIds);

           // var groupedReporters = reporters.ToLookup(x => x.Id);
         
          //  var groupedPosts = posts.ToLookup(x => x.Topic.Id);
            // Gets posts for topics
            var postService = ServiceFactory.Get<IPostService>();
          //  var posts = postService.GetPosts(postIds);
         //   var groupedPosts = posts.ToLookup(x => x.Id);

            var viewModels = new List<ReportViewModel>();
            
             foreach (var report in reports)
            {
             //   var id = report.ReferenceId;
                // var id = report.Reporter.Id;
                //var topicPosts
                //   var postReports = (groupedReporters.Contains(id) ? groupedReporters[id].ToList() : new List<MembershipUser>());
                // viewModels.Add(CreateReportViewModel(report, postReports));

               // var postReported = (groupedPosts.Contains(id) ? groupedPosts[id].ToList() : new List<Post>());
              //  viewModels.Add(CreateReportViewModel(report, postReported));
              //  viewModels.Add(CreateTopicViewModel(topic, permission, topicPosts, null, null, null, null, loggedOnUser, settings));
            }
            
            return viewModels;
        }
        public static ReportViewModel CreateReportViewModel(Report report, List<Post> postReported) {
            var viewModel = new ReportViewModel
            {
                Report = report,
                PostReported = postReported

            };
          //  viewModel.PostReported = CreatePostViewModels(postReported, votes, permission, topic, loggedOnUser, settings, allFavourites);
            return viewModel;
        }
    public static List<TopicViewModel> CreateTopicViewModels(List<Topic> topics,
                                                                IRoleService roleService,
                                                                MembershipRole usersRole,
                                                                MembershipUser loggedOnUser,
                                                                List<Category> allowedCategories,
                                                                Settings settings)
        {
            // Get all topic Ids
            var topicIds = topics.Select(x => x.Id).ToList();

            // Gets posts for topics
            var postService = ServiceFactory.Get<IPostService>();
            var posts = postService.GetPostsByTopics(topicIds, allowedCategories);
            var groupedPosts = posts.ToLookup(x => x.Topic.Id);

            // Get all permissions
            var permissions = GetPermissionsForTopics(topics, roleService, usersRole);

            // Create the view models
            var viewModels = new List<TopicViewModel>();
            foreach (var topic in topics)
            {
                var id = topic.Id;
                var permission = permissions[topic.Category];
                var topicPosts = (groupedPosts.Contains(id) ? groupedPosts[id].ToList() : new List<Post>());
                viewModels.Add(CreateTopicViewModel(topic, permission, topicPosts, null, null, null, null, loggedOnUser, settings));
            }
            return viewModels;
        }

        public static TopicViewModel CreateTopicViewModel(Topic topic,
                                                    PermissionSet permission,
                                                    List<Post> posts,
                                                    Post starterPost,
                                                    int? pageIndex,
                                                    int? totalCount,
                                                    int? totalPages,
                                                    MembershipUser loggedOnUser,
                                                    Settings settings,
                                                    bool getExtendedData = false)
        {

            var topicNotificationService = ServiceFactory.Get<ITopicNotificationService>();
            var pollAnswerService = ServiceFactory.Get<IPollAnswerService>();
            var voteService = ServiceFactory.Get<IVoteService>();
            var favouriteService = ServiceFactory.Get<IFavouriteService>();

            var userIsAuthenticated = loggedOnUser != null;

            // Check for online status
            var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);

            var viewModel = new TopicViewModel
            {
                Permissions = permission,
                Topic = topic,
                Views = topic.Views,
                DisablePosting = loggedOnUser != null && (loggedOnUser.DisablePosting == true),
                PageIndex = pageIndex,
                TotalCount = totalCount,
                TotalPages = totalPages,
                LastPostPermaLink = string.Concat(topic.NiceUrl, "?", AppConstants.PostOrderBy, "=", AppConstants.AllPosts, "#comment-", topic.LastPost.Id),
                MemberIsOnline = topic.User.LastActivityDate > date,
            };

            if (starterPost == null)
            {
                starterPost = posts.FirstOrDefault(x => x.IsTopicStarter);
            }

            // Get votes for all posts
            var postIds = posts.Select(x => x.Id).ToList();
            postIds.Add(starterPost.Id);

            // Get all votes by post
            var votes = voteService.GetVotesByPosts(postIds);

            // Get all favourites for this user
            var allFavourites = favouriteService.GetByTopic(topic.Id);

            // Map the votes
            var startPostVotes = votes.Where(x => x.Post.Id == starterPost.Id).ToList();
            var startPostFavs = allFavourites.Where(x => x.Post.Id == starterPost.Id).ToList();

            // Create the starter post viewmodel
            viewModel.StarterPost = CreatePostViewModel(starterPost, startPostVotes, permission, topic, loggedOnUser, settings, startPostFavs);

            // Map data from the starter post viewmodel
            viewModel.VotesUp = startPostVotes.Count(x => x.Amount > 0);
            viewModel.VotesDown = startPostVotes.Count(x => x.Amount < 0);
            viewModel.Answers = totalCount != null ? (int)totalCount : posts.Count() - 1;

            // Create the ALL POSTS view models
            viewModel.Posts = CreatePostViewModels(posts, votes, permission, topic, loggedOnUser, settings, allFavourites);

            // ########### Full topic need everything   

            if (getExtendedData)
            {
                // See if the user has subscribed to this topic or not
                var isSubscribed = userIsAuthenticated && (topicNotificationService.GetByUserAndTopic(loggedOnUser, topic).Any());
                viewModel.IsSubscribed = isSubscribed;

                // See if the topic has a poll, and if so see if this user viewing has already voted
                if (topic.Poll != null)
                {
                    // There is a poll and a user
                    // see if the user has voted or not

                    viewModel.Poll = new PollViewModel
                    {
                        Poll = topic.Poll,
                        UserAllowedToVote = permission[SiteConstants.Instance.PermissionVoteInPolls].IsTicked
                    };

                    var answers = pollAnswerService.GetAllPollAnswersByPoll(topic.Poll);
                    if (answers.Any())
                    {
                        var pollvotes = answers.SelectMany(x => x.PollVotes).ToList();
                        if (userIsAuthenticated)
                        {
                            viewModel.Poll.UserHasAlreadyVoted = (pollvotes.Count(x => x.User.Id == loggedOnUser.Id) > 0);
                        }
                        viewModel.Poll.TotalVotesInPoll = pollvotes.Count();
                    }
                }
            }

            return viewModel;
        }
        #endregion

        #region Post
        public static PostViewModel CreatePostViewModel(Post post, List<Vote> votes, PermissionSet permission, Topic topic, MembershipUser loggedOnUser, Settings settings, List<Favourite> favourites)
        {
           var reportService = ServiceFactory.Get<IReportService>();
            var allowedToVote = (loggedOnUser != null && loggedOnUser.Id != post.User.Id);
           

            if (allowedToVote && settings.EnablePoints)
            {
                // We need to check if points are enabled that they have enough points to vote
                allowedToVote = loggedOnUser.TotalPoints >= settings.PointsAllowedToVoteAmount;
            }

            // Remove votes where no VotedBy has been recorded
            votes.RemoveAll(x => x.VotedByMembershipUser == null);

            var hasVotedUp = false;
            var hasVotedDown = false;
            var hasFavourited = false;
          //  var isreported = false;
            var hasreport = false;

            if (loggedOnUser != null && loggedOnUser.Id != post.User.Id)
            {

                hasreport = reportService.PostReports(loggedOnUser, post.Id).Any();
            }
            if (loggedOnUser != null && loggedOnUser.Id != post.User.Id)
            {              
             //   isreported = reportService.GetByUserAndReference(loggedOnUser, post.Id).Any();

                hasFavourited = favourites.Any(x => x.Member.Id == loggedOnUser.Id);
                hasVotedUp = votes.Count(x => x.Amount > 0 && x.VotedByMembershipUser.Id == loggedOnUser.Id) > 0;
                hasVotedDown = votes.Count(x => x.Amount < 0 && x.VotedByMembershipUser.Id == loggedOnUser.Id) > 0;
            }

            // Check for online status
            var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);

            return new PostViewModel
            {
                Permissions = permission,
                Votes = votes,
                Post = post,
                ParentTopic = topic,
                AllowedToVote = allowedToVote,
                MemberHasFavourited = hasFavourited,
                Favourites = favourites,
                PermaLink = string.Concat(topic.NiceUrl, "?", AppConstants.PostOrderBy, "=", AppConstants.AllPosts, "#comment-", post.Id),
                MemberIsOnline = post.User.LastActivityDate > date,
                HasVotedDown = hasVotedDown,
                HasVotedUp = hasVotedUp,
                HasReport = hasreport,
                ShowTopicName = false

            };
        }

        /// <summary>
        /// Maps the posts for a specific topic
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="votes"></param>
        /// <param name="permission"></param>
        /// <param name="topic"></param>
        /// <param name="loggedOnUser"></param>
        /// <param name="settings"></param>
        /// <param name="favourites"></param>
        /// <returns></returns>
        public static List<PostViewModel> CreatePostViewModels(IEnumerable<Post> posts, List<Vote> votes, PermissionSet permission, Topic topic, MembershipUser loggedOnUser, Settings settings, List<Favourite> favourites)
        {
          //  var reportService = ServiceFactory.Get<IReportService>();
            var viewModels = new List<PostViewModel>();
            var groupedVotes = votes.ToLookup(x => x.Post.Id);
            var groupedFavourites = favourites.ToLookup(x => x.Post.Id);
            foreach (var post in posts)
            {
                var id = post.Id;        
                var postVotes = (groupedVotes.Contains(id) ? groupedVotes[id].ToList() : new List<Vote>());
                var postFavs = (groupedFavourites.Contains(id) ? groupedFavourites[id].ToList() : new List<Favourite>());
                viewModels.Add(CreatePostViewModel(post, postVotes, permission, topic, loggedOnUser, settings, postFavs));
            }
            return viewModels;
        }

        /// <summary>
        /// Maps posts from any topic must be pre filtered by checked the user has access to them
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="votes"></param>
        /// <param name="permissions"></param>
        /// <param name="loggedOnUser"></param>
        /// <param name="settings"></param>
        /// <param name="favourites"></param>
        /// <returns></returns>
        public static List<PostViewModel> CreatePostViewModels(IEnumerable<Post> posts, List<Vote> votes, Dictionary<Category, PermissionSet> permissions, MembershipUser loggedOnUser, Settings settings, List<Favourite> favourites)
        {
            var viewModels = new List<PostViewModel>();
            var groupedVotes = votes.ToLookup(x => x.Post.Id);
            var groupedFavourites = favourites.ToLookup(x => x.Post.Id);
            foreach (var post in posts)
            {
                var id = post.Id;
                var topic = post.Topic;
                var permission = permissions[topic.Category];
                var postVotes = (groupedVotes.Contains(id) ? groupedVotes[id].ToList() : new List<Vote>());
                var postFavs = (groupedFavourites.Contains(id) ? groupedFavourites[id].ToList() : new List<Favourite>());
                viewModels.Add(CreatePostViewModel(post, postVotes, permission, topic, loggedOnUser, settings, postFavs));

              //  var viewModel = CreatePostViewModel(post, postVotes, permission, topic, loggedOnUser, settings, postFavs);
              //  viewModel.MinimalPost = true;
              //  viewModel.ShowTopicName = true;
              //  viewModels.Add(viewModel);
            }
            return viewModels;
        }
        #endregion

      

        #region Category

        public static Dictionary<Category, PermissionSet> GetPermissionsForCategories(IEnumerable<Category> categories, IRoleService roleService, MembershipRole usersRole)
        {
            // Permissions
            // loop through the categories and get the permissions
            var permissions = new Dictionary<Category, PermissionSet>();
            foreach (var category in categories)
            {
                var permissionSet = roleService.GetPermissions(category, usersRole);
                permissions.Add(category, permissionSet);
            }
            return permissions;
        }

        public static Dictionary<CategorySummary, PermissionSet> GetPermissionsForCategories(IEnumerable<CategorySummary> categories,
           IRoleService roleService, MembershipRole usersRole, bool removeIfDenyAccess = false)
        {
            // Permissions
            // loop through the categories and get the permissions
            var permissions = new Dictionary<CategorySummary, PermissionSet>();
            foreach (var summary in categories)
            {
                var permissionSet = roleService.GetPermissions(summary.Category, usersRole);

                // Should we add if deny access is ticked
                if (removeIfDenyAccess)
                {
                    // See if deny access is ticked
                 /*   if (permissionSet[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                    {
                        continue;
                    }*/
                }

                permissions.Add(summary, permissionSet);
            }
            return permissions;
        }

        #endregion

        #region Chat Room

     public static List<ChatRoomViewModel> CreateChatRoomViewModels(IList<ChatRoom> chatRooms, MembershipUser loggedOnUser, Settings settings)
        {
            // Get all chatRooms Ids
            var chatRoomsIds = chatRooms.Select(x => x.Id).ToList();

            // Gets posts for topics
            var chatService = ServiceFactory.Get<IChatService>();
            var viewModels = new List<ChatRoomViewModel>();

            foreach (var chatRoom in chatRooms)
            {
                var id = chatRoom.Id;
                viewModels.Add(CreateChatRoomViewModel(chatRoom, loggedOnUser, settings));
            }
            return viewModels;
        }

        public static ChatRoomViewModel CreateChatRoomViewModel(ChatRoom chatRoom, MembershipUser loggedOnUser, Settings settings)
        {
            var chatService = ServiceFactory.Get<IChatService>();
            var userIsAuthenticated = loggedOnUser != null;                 
            var viewModel = new ChatRoomViewModel
            {
                ChatRoom = chatRoom,
				ShowUnSubscribedLink = false,
            IsSubscribed = userIsAuthenticated && (chatService.GetRoomSubscription(loggedOnUser, chatRoom).Any())
			//   DisableChat = loggedOnUser != null && (loggedOnUser.DisablePosting == true),
			};
            // See if the user has subscribed to this room or not
    //   var isSubscribed = userIsAuthenticated && (chatService.GetRoomSubscription(loggedOnUser, chatRoom).Any());
         //   viewModel.IsSubscribed = isSubscribed;
            return viewModel;
        }


        public static ChatRoomViewModel CreateChatRoomViewModel(ChatRoom chatRoom,List<ChatMessage> chatMessages, int? pageIndex, int? totalCount,int? totalPages, MembershipUser loggedOnUser, Settings settings)
        {
            var chatService = ServiceFactory.Get<IChatService>();
            var userIsAuthenticated = loggedOnUser != null;
            var messageIds = chatMessages.Select(x => x.Id).ToList();
            // Gets reactions for messages
          
            var reactions = chatService.GetResponses(messageIds);
            var membersReaction = reactions.ToLookup(x => x.ChatMessage.Id);

            var viewModel = new ChatRoomViewModel
            {
                ChatRoom = chatRoom,
              //  DisableChat = loggedOnUser != null && (loggedOnUser.DisableChat == true),
                PageIndex = pageIndex,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IsSubscribed = userIsAuthenticated && (chatService.GetRoomSubscription(loggedOnUser, chatRoom).Any()),
            ChatMessages =  CreateMessageViewModels(chatMessages, loggedOnUser)
        };        
            return viewModel;
        }

        #endregion

        #region Chat Message
        public static ChatMessagesViewModel CreateChatMessagesViewModel(ChatRoom chatRoom, List<ChatMessage> chatMessages, int pageIndex, int? totalCount, int? totalPages, MembershipUser loggedOnUser)
        {
            var chatService = ServiceFactory.Get<IChatService>();
            var userIsAuthenticated = loggedOnUser != null;
            var messageIds = chatMessages.Select(x => x.Id).ToList();
            // Gets reactions for messages

            var reactions = chatService.GetResponses(messageIds);
            var membersReaction = reactions.ToLookup(x => x.ChatMessage.Id);

            var viewModel = new ChatMessagesViewModel
            {
                ChatRoom = chatRoom,
                PageIndex = pageIndex,
                TotalCount = totalCount,
                TotalPages = totalPages,
                ChatMessages = CreateMessageViewModels(chatMessages, loggedOnUser)
            };
            return viewModel;
        }
       
        public static List<ChatMessageViewModel> CreateMessageViewModels(List<ChatMessage> chatMessages,MembershipUser loggedOnUser)
        {
            // Get all message Ids
            var messageIds = chatMessages.Select(x => x.Id).ToList();
            // Gets reactions for messages
            var chatService = ServiceFactory.Get<IChatService>();
            var reactions = chatService.GetResponses(messageIds);
            var membersReaction = reactions.ToLookup(x => x.ChatMessage.Id);

            var viewModels = new List<ChatMessageViewModel>();
         

            foreach (var message in chatMessages)
            {
                var id = message.Id;
                var messageReactions = (membersReaction.Contains(id) ? membersReaction[id].ToList() : new List<ChatResponse>());
                viewModels.Add(CreateMessageViewModel(message, messageReactions, loggedOnUser));
            }
            return viewModels;
        }
       
        public static ChatMessageViewModel CreateMessageViewModel(ChatMessage chatMessage, List<ChatResponse> chatResponses, MembershipUser loggedOnUser)

        {
            var viewModel = new ChatMessageViewModel();
            var chatService = ServiceFactory.Get<IChatService>();
            var allowedToResponse = false;
            var enableDelete = false;
            var enableReplay = false;
            var messageReplies = chatService.GetReplies(chatMessage.Id);
            var logged_on = false;
         
            if (loggedOnUser != null)
            {
                var datefrom = DateTime.UtcNow.AddDays(-AppConstants.TimeSpanMonthDays);
                var spamReports = chatService.GetReports(loggedOnUser.Id, datefrom);
             
                //    if (loggedOnUser.Id != chatMessage.User.Id)
               // {
                    var subscription = chatService.GetRoomSubscription(loggedOnUser.Id, chatMessage.ChatRoom.Id);

                // We need to check if points are enabled that they have enough points to response
                /*                    if (loggedOnUser.TotalPoints >= settings.PointsAllowedToVoteAmount && settings.EnablePoints)*/
     
                    if (subscription != null && chatMessage.Deleted != true)
                        {
                            if (subscription.LogOn)
                            {
                                if (spamReports.Count < SiteConstants.Instance.ReportCountToDisable)
                                {
                                    if (loggedOnUser.Id == chatMessage.User.Id)
                                    {
                                        enableDelete = true;
                                    }
                                    if (chatMessage.User.Id != loggedOnUser.Id)
                                    {
                                        allowedToResponse = true;
                                        enableReplay = true;

                                    }
                                }
                      //  logged_on = true;
                      /*  TimeSpan span = DateTime.Now - subscription.LastActive;
                        if (span.TotalSeconds < 15)
                        {
                            logged_on = true;
                        }  */               
                    }
                    logged_on = subscription.LogOn;
                        }
                     
                   // }
               // }
             
                }     
                chatResponses.RemoveAll(x => x.MembershipUserRespond == null);
			         
            var has_favourite = false;        
            var has_report = false;
            if (loggedOnUser != null && loggedOnUser.Id != chatMessage.User.Id)
            {           
      has_favourite = chatResponses.Count(x => x.Amount > 0 && x.MembershipUserRespond.Id == loggedOnUser.Id) >0;
      has_report = chatResponses.Count(x => x.Amount < 0 && x.MembershipUserRespond.Id == loggedOnUser.Id) > 0;
            }
			if(has_favourite || has_report)
                {
				allowedToResponse = false;
				}
				
            // Check for online status
            var date = DateTime.UtcNow.AddMinutes(-AppConstants.TimeSpanInMinutesToShowMembers);
          return  new ChatMessageViewModel
             {
           ChatMessage = chatMessage,
            ChatResponses = chatResponses,
            User = loggedOnUser,
            AllowedToResponse = allowedToResponse,
              EnableDelete = enableDelete,
              HasFavourite = has_favourite,
              HasReport = has_report,
           MemberIsOnline = chatMessage.User.LastActivityDate > date,
           Replies = messageReplies.Count,
           EnableReplay = enableReplay,
           LogOn = logged_on
          };       
        }
        #endregion    
    }
}
