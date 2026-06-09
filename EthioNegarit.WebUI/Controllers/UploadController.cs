namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Areas.Admin.ViewModels;
    using ViewModels;
    using Core.Domain.Entities;
    using System.Web.Hosting;
    using Application;
    using System.IO;
    using Core.Domain.General;
    public partial class UploadController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IUploadedFileService _uploadedFileService;
        private readonly IAdvertFileService _advertFileService;
        private readonly IAdService _adService;
        public UploadController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IAdService adService, IAdvertFileService advertFileService,
            IPostService postService, IUploadedFileService uploadedFileService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _postService = postService;
            _uploadedFileService = uploadedFileService;
            _adService = adService;
            _advertFileService = advertFileService;
        }
        [HttpPost]
        public ActionResult UploadPostFiles(AttachFileToPostViewModel attachFileToPostViewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var topic = new Topic();

                try
                {

                    // First this to do is get the post
                    var post = _postService.Get(attachFileToPostViewModel.UploadPostId);
                    if (post != null)
                    {
                        // Now get the topic
                        topic = post.Topic;

                        // Check we get a valid post back and have some file
                        if (attachFileToPostViewModel.Files != null && attachFileToPostViewModel.Files.Any())
                        {
                            // Now get the category
                            var category = topic.Category;

                            // Get the permissions for this category, and check they are allowed to update and 
                            // not trying to be a sneaky mofo
                            var permissions = RoleService.GetPermissions(category, UsersRole);
                            if (permissions[SiteConstants.Instance.PermissionAttachFiles].IsTicked == false || LoggedOnReadOnlyUser.DisableFileUploads == true)
                            {
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                                    MessageType = GenericMessages.danger
                                };

                                return Redirect(topic.NiceUrl);
                            }

                            // woot! User has permission and all seems ok
                            // Before we save anything, check the user already has an upload folder and if not create one
  var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, LoggedOnReadOnlyUser.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            foreach (var file in attachFileToPostViewModel.Files)
                            {
                                if (file != null)
                                {
                                    // If successful then upload the file
        var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService,true);
                                    if (!uploadResult.UploadSuccessful)
                                    {
                                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                        {
                                            Message = uploadResult.ErrorMessage,
                                            MessageType = GenericMessages.danger
                                        };
                                        return Redirect(topic.NiceUrl);
                                    }

                                    // Add the filename to the database
                                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                                    var uploadedFile = new UploadedFile
                                    {
                                        Filename = uploadResult.UploadedFileName,
                                        Post = post,
                                        MembershipUser = loggedOnUser
                                    };
                                    _uploadedFileService.Add(uploadedFile);

                                }
                            }

                            //Commit
                            unitOfWork.Commit();

                            // Redirect to the topic with a success message
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Post.FilesUploaded"),
                                MessageType = GenericMessages.success
                            };

                            return Redirect(topic.NiceUrl);

                        }
                        // Else return with error to home page
                        return topic != null ? Redirect(topic.NiceUrl) : ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                    // Else return with error to home page
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                        MessageType = GenericMessages.danger
                    };
                    return topic != null ? Redirect(topic.NiceUrl) : ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
        }
        public ActionResult DeleteUploadedFile(Guid id)
        {
            if (id != Guid.Empty)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    Topic topic = null;
                    try
                    {
                        // Get the file and associated objects we'll need
                        var uploadedFile = _uploadedFileService.Get(id);
                        var post = uploadedFile.Post;
                        topic = post.Topic;

                        if (UsersRole.RoleName == AppConstants.AdminRoleName || uploadedFile.MembershipUser.Id == LoggedOnReadOnlyUser.Id)
                        {
                            // Ok to delete file
                            // Remove it from the post
                            post.Files.Remove(uploadedFile);

                            // store the file path as we'll need it to delete on the file system
                            var filePath = uploadedFile.FilePath;

                            // Now delete it
                            _uploadedFileService.Delete(uploadedFile);


                            // And finally delete from the file system
                            System.IO.File.Delete(HostingEnvironment.MapPath(filePath));
                        }
                        else
                        {
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                                MessageType = GenericMessages.danger
                            };
                            Redirect(topic.NiceUrl);
                        }

                        //Commit
                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("File.SuccessfullyDeleted"),
                            MessageType = GenericMessages.success
                        };
                        return Redirect(topic.NiceUrl);
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        };
                        return topic != null ? Redirect(topic.NiceUrl) : ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
        [HttpPost]
        public ActionResult UploadAdFiles(UploadAdFileViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // Add the filename to the database
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    // First to do this get Ad        
                    var advert = _adService.Get(viewModel.advertId);
                    if (advert != null)
                    {
                        // Check we get a valid post back and have some file
                        /*  if (viewModel.Files != null && viewModel.Files.Any())*/
                        if (viewModel.Files != null)
                        {
                            // Get the permissions for this category, and check they are allowed to update and 
                            // not trying to be a sneaky mofo
                            // var permissions = RoleService.GetPermissions(category, UsersRole);
                            // if (permissions[SiteConstants.Instance.PermissionAttachFiles].IsTicked == false || LoggedOnReadOnlyUser.DisableFileUploads == true)
                            /*  if (LoggedOnReadOnlyUser.DisableFileUploads == true)
                              {
                                  TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                  {
                                      Message = LocalizationService.GetResourceString("Errors.NoPermission"),
                                      MessageType = GenericMessages.danger
                                  };
                               //   return RedirectToAction("List", "Ad");
                                  return RedirectToAction("List", "Ad", new { area = string.Empty });
                              }*/
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, advert.Id));

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }
                            // Loop through each file and get the file info and save to the users folder and Db
                            foreach (var file in viewModel.Files)
                            {
                                if (file != null)
                                {
                                    var filename = Path.GetFileName(file.FileName);
                                    filename = filename.ToLower();
                                    var fileExtension = Path.GetExtension(filename);
                                    fileExtension = fileExtension.ToLower().TrimStart('.');
                                    if (fileExtension == "mp4")
                                    {
                                        //Check If Any Video and Remove
                                        var videoFile = _adService.GetVideo(advert.Id);
                                        if (videoFile != null)
                                        {
                                            // Remove it from the Ad
                                            advert.Files.Remove(videoFile);
                                            // store the file path as we'll need it to delete on the file system
                                            var filePath = videoFile.FilePath;
                                            // Now delete it
                                            _advertFileService.Delete(videoFile);
                                            // And finally delete from the file system
                                            System.IO.File.Delete(HostingEnvironment.MapPath(filePath));
                                        }

                                        //Now Add the new Video File
                                        byte[] bytes;
                                        using (BinaryReader br = new BinaryReader(file.InputStream))
                                        {
                                            bytes = br.ReadBytes(file.ContentLength);
                                        }
                                        // Add the file to the database
                                        var advertisementFile = new AdvertisementFile
                                        {
                                            Filename = Path.GetFileName(file.FileName),
                                            Advertisement = advert,
                                            MembershipUser = loggedOnUser,
                                            FileType = "mp4",
                                            Data = bytes,
                                            ContentType = file.ContentType
                                        };
                                        _advertFileService.Add(advertisementFile);
                                    }
                                    else
                                    {
                                        // If successful then upload the file
                                        var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, LocalizationService, true);
                                        if (!uploadResult.UploadSuccessful)
                                        {
                                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                            {
                                                Message = uploadResult.ErrorMessage,
                                                MessageType = GenericMessages.danger
                                            };
                                            return RedirectToAction("MyAds", "Ad", new { area = string.Empty });
                                        }

                                        // Add the filename to the database
                                        var advertisementFile = new AdvertisementFile
                                        {
                                            Filename = uploadResult.UploadedFileName,
                                            Advertisement = advert,
                                            MembershipUser = loggedOnUser,

                                        };
                                        _advertFileService.Add(advertisementFile);
                                    }
                                }
                            }
                            unitOfWork.Commit();
                            // Redirect to the Ad List with a success message
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Post.FilesUploaded"),
                                MessageType = GenericMessages.success
                            };
                            return RedirectToAction("MyAds", "Ad");
                        }
                        return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
        }
    }
}