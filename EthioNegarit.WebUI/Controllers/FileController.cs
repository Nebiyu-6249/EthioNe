namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Linq;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using ViewModels;
    using System.Web.Hosting;
    using Application;
    using Core.Constants;
    using Areas.Admin.ViewModels;
    using System.IO;
    using System.IO.Compression;

    public partial class FileController : BaseController
    {
        private readonly IUploadedFileService _uploadedFileService;
        private readonly ICategoryService _categoryService;
        private readonly IAdvertFileService _advertFileService;
        private readonly IAdService _adService;
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IAttachmentService _attachmentService;
        public FileController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IAdService adService, IAdvertFileService advertFileService, IPrivateMessageService privateMessageService, IAttachmentService attachmentService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IUploadedFileService uploadedFileService, ICategoryService categoryService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _uploadedFileService = uploadedFileService;
            _categoryService = categoryService;
            _adService = adService;
            _advertFileService = advertFileService;
            _privateMessageService = privateMessageService;
            _attachmentService = attachmentService;
        }
        public FileResult Download(Guid id)
        {
            var uploadedFileById = _uploadedFileService.Get(id);
            if (uploadedFileById != null)
            {
                // Check the user has permission to download this file
                var fileCategory = uploadedFileById.Post.Topic.Category;
                var allowedCategoryIds = _categoryService.GetAllowedCategories(UsersRole).Select(x => x.Id);
                if (allowedCategoryIds.Contains(fileCategory.Id))
                {
                    //if(AppHelpers.FileIsImage(uploadedFileById.FilePath))
                    //{

                    //}
                    var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(uploadedFileById.FilePath));
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, uploadedFileById.Filename);
                }
            }
            return null;
        }
        [HttpGet]
        public EmptyResult VideoStream(Guid adId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var videoFile = _adService.GetVideo(adId);
                var viewModel = new VideoViewModel
                {
                    FileName = videoFile.Filename,
                    FileType = videoFile.FileType,
                    Id = videoFile.Id,
                    Data = videoFile.Data
                };
                HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + viewModel.FileName + "." + viewModel.FileType);
                HttpContext.Response.BinaryWrite(viewModel.Data);
                return new EmptyResult(); ;
            }
        }
        public FileResult DownloadAdFile(Guid id)
        {
            var uploadedFileById = _advertFileService.Get(id);
            if (uploadedFileById != null)
            {
                if (AppHelpers.FileIsImage(uploadedFileById.FilePath))
                {
                    var filePath = uploadedFileById.FilePath;
                    var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(filePath));
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, uploadedFileById.Filename);
                }
                else
                {
                    var viewModel = new VideoViewModel
                    {
                        FileName = uploadedFileById.Filename,
                        FileType = uploadedFileById.FileType,
                        Id = uploadedFileById.Id,
                        Data = uploadedFileById.Data
                    };
                    HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + viewModel.FileName + "." + viewModel.FileType);
                    HttpContext.Response.BinaryWrite(viewModel.Data);
                }
            }
            return null;
        }
        public ActionResult DeleteAdFile(Guid id)
        {
            if (id != Guid.Empty)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get the file and associated objects we'll need
                        var advertFile = _advertFileService.Get(id);
                        var advert = advertFile.Advertisement;
                        if (UsersRole.RoleName == AppConstants.AdminRoleName || advertFile.MembershipUser.Id == LoggedOnReadOnlyUser.Id)
                        {
                            // Ok to delete file
                            // Remove it from the Ad
                            advert.Files.Remove(advertFile);
                            // store the file path as we'll need it to delete on the file system
                            var filePath = advertFile.FilePath;
                            // Now delete it
                            _advertFileService.Delete(advertFile);
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
                            return RedirectToAction("MyAds", "Ad");
                        }
                        //Commit
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("File.SuccessfullyDeleted"),
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("MyAds", "Ad");
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
                        return RedirectToAction("MyAds", "Ad");
                    }
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
        public ActionResult Attachments(Guid messageId)
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                 {
                    var message_Id =  Guid.Empty;;
                var privateMessage = _privateMessageService.Get(messageId);
                if (privateMessage.IsSentMessage != true) {
                        message_Id = messageId;                                         
                    }
                    else
                    {
                        var message = _privateMessageService.GetMatchingPrivateMessage(privateMessage.DateSent, privateMessage.UserFrom.Id, privateMessage.UserTo.Id);
                        message_Id = message.Id;
                    }              
                var attachements = _attachmentService.GetAttachments(message_Id);
                var viewModel = new PrivateMessageAttachementsViewModel
                    {
                        Attachments = attachements,
                       PrivateMessage = privateMessage,
                    MessageId = message_Id
                };
                    return PartialView(viewModel);
               }
            }
            return null;
        }
        public void DeleteAttachments(Guid messageId)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Get private message

                var privateMessage = _privateMessageService.Get(messageId);
                if (privateMessage.UserFrom.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                {
                    var messageAttachements = _attachmentService.GetAttachments(messageId);
                    if (messageAttachements != null)
                    {
                        //  var attachements = new List<Attachment>();
                        //  attachements.AddRange(privateMessage.Attachments);
                        foreach (var attachement in messageAttachements)
                        {
                            // Now delete it
                            var filePath = attachement.FilePath;
                            // Now delete it
                            _attachmentService.Delete(attachement);

                            // And finally delete from the file system
                            System.IO.File.Delete(HostingEnvironment.MapPath(filePath));
                        }
                        privateMessage.Attachments.Clear();
                        unitOfWork.SaveChanges();
                    }
                    try
                    {
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Post.Deleted"),
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });

                    }
                }
            }
        }
        [HttpPost]
        public void DeleteAttachment(Guid id)
        {
            if (id != Guid.Empty)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get the file and associated objects we'll need
                        var attachement = _attachmentService.Get(id);
                        var message = attachement.PrivateMessage;
                        if (UsersRole.RoleName == AppConstants.AdminRoleName || attachement.MembershipUser.Id == LoggedOnReadOnlyUser.Id)
                        {
                            // Ok to delete file
                            // Remove it from the Message
                            message.Attachments.Remove(attachement);

                            // store the file path as we'll need it to delete on the file system
                            var filePath = attachement.FilePath;

                            // Now delete it
                            _attachmentService.Delete(attachement);

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
                            //   return RedirectToAction("Index");
                        }
                        //Commit
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("File.SuccessfullyDeleted"),
                            MessageType = GenericMessages.success
                        };

                        // return RedirectToAction("Index");
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
                        //  return topic != null ? Redirect(topic.NiceUrl) : ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            //  return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
        public FileResult DownloadAttachment(Guid id)
        {
            var uploadedFileById = _attachmentService.GetAttachment(id);
            if (uploadedFileById != null)
            {
                var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(uploadedFileById.FilePath));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, uploadedFileById.Filename);
            }
            return null;
        }
        public ActionResult DownloadAttachments(Guid messageId)
        {
            var messageAttachements = _attachmentService.GetAttachments(messageId);
            if (messageAttachements.Count > 1) {
            using (var memoryStream = new MemoryStream())
            {
                //Standard Library: Use the System.IO.Compression.ZipArchive class provided by .NET.
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    //   var messageAttachements = _attachmentService.GetAttachments(messageId);
                    foreach (var attachement in messageAttachements)
                    {
                        //  var file = GetFileFromDatabase(id); // Your logic to get file data
                        var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(attachement.FilePath));
                        var file = File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, attachement.Filename);
                        var entry = zipArchive.CreateEntry(attachement.Filename);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(file.FileContents, 0, file.FileContents.Length);
                        }
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "files.zip");
            }
            }
            else
            {
               var attachement =  messageAttachements.FirstOrDefault();
                var fileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(attachement.FilePath));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, attachement.Filename);
            }
        }  
        public PartialViewResult ImageUploadTinyMce()
        {           // Testing
            return PartialView();
        }
    }
}