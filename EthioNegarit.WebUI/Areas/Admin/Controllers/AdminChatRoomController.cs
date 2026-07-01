using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using EthioNegarit.Core;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Application;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using EthioNegarit.Services.Data.UnitOfWork;

namespace EthioNegarit.WebUI.Areas.Admin.Controllers
{
    
    public class AdminChatRoomController : BaseAdminController
    {
        // GET: Admin/ChatRoom
        private readonly IChatService _chatService;
        public AdminChatRoomController(ILoggingService loggingService,
                                      IUnitOfWorkManager unitOfWorkManager,
                                     IMembershipService membershipService,
                                     ILocalizationService localizationService,
                                     IChatService chatService,
                                     ISettingsService settingsService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
        {
            _chatService = chatService;
        }
        // GET: Admin/AdminChat
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public PartialViewResult GetChatRooms()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListChatRoomsViewModel
                {
                    ChatRooms = _chatService.GetChatRooms().OrderBy(x => x.Name)
                };
                return PartialView(viewModel);
            }
        }

        public ActionResult CreateChatRoom()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var categoryViewModel = new AdminChatRoomViewModel();
                return View(categoryViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChatRoom(AdminChatRoomViewModel chatRoomViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var chatRoom = new ChatRoom
                        {
                            Name = chatRoomViewModel.Name,
                            Description = chatRoomViewModel.Description,
                            IsLocked = chatRoomViewModel.IsLocked,
                            Freeze = false,
                            PageTitle = chatRoomViewModel.PageTitle,
                            MetaDescription = chatRoomViewModel.MetaDesc,
                            Colour = chatRoomViewModel.Colour
                        };

                        // Sort image out first
                        if (chatRoomViewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, chatRoom.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = chatRoomViewModel.Files[0];
                            if (file != null)
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
                                    return View(chatRoomViewModel);
                                }

                                // Save avatar to user
                                chatRoom.Image = uploadResult.UploadedFileName;
                            }

                        }

                        _chatService.Add(chatRoom);

                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Chat Room Created",
                            MessageType = GenericMessages.success

                        };
                        unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the category");
            }

            return RedirectToAction("Index");
        }


        private AdminChatRoomViewModel CreateEditChatRoomViewModel(ChatRoom chatRoom)
        {
            var chatRoomViewModel = new AdminChatRoomViewModel
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name,
                PageTitle = chatRoom.PageTitle,
                MetaDesc = chatRoom.MetaDescription,
                Description = chatRoom.Description,
                IsLocked = chatRoom.IsLocked,
                Freeze = (bool)chatRoom.Freeze,
                Image = chatRoom.Image,
                Colour = chatRoom.Colour,

            };
            return chatRoomViewModel;
        }

        public ActionResult EditChatRoom(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var chatRoom = _chatService.Get(id);
                var chatRoomViewModel = CreateEditChatRoomViewModel(chatRoom);

                return View(chatRoomViewModel);
            }
        }

        [HttpPost]
        public ActionResult EditChatRoom(AdminChatRoomViewModel chatRoomViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {

                        var chatRoom = _chatService.Get(chatRoomViewModel.Id);

                        // Sort image out first
                        if (chatRoomViewModel.Files != null)
                        {
                            // Before we save anything, check the user already has an upload folder and if not create one
                            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, chatRoomViewModel.Id));
                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            // Loop through each file and get the file info and save to the users folder and Db
                            var file = chatRoomViewModel.Files[0];
                            if (file != null)
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
                                    return View(chatRoomViewModel);
                                }

                                // Save avatar to user
                                chatRoom.Image = uploadResult.UploadedFileName;
                            }

                        }


                        chatRoom.Description = chatRoomViewModel.Description;
                        chatRoom.IsLocked = chatRoomViewModel.IsLocked;
                        chatRoom.Name = chatRoomViewModel.Name;
                        chatRoom.PageTitle = chatRoomViewModel.PageTitle;
                        chatRoom.MetaDescription = chatRoomViewModel.MetaDesc;
                        chatRoom.Colour = chatRoomViewModel.Colour;
                        chatRoom.Freeze = chatRoomViewModel.Freeze;
                        _chatService.UpdateSlugFromName(chatRoom);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Chat Room Updated",
                            MessageType = GenericMessages.success
                        };

                        chatRoomViewModel = CreateEditChatRoomViewModel(chatRoom);

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Update Failed",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            return View(chatRoomViewModel);
        }



        public ActionResult DeleteChatRoomConfirmation(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var chatRoom = _chatService.Get(id);
                //  var subCats = _categoryService.GetAllSubCategories(id).ToList();
                var viewModel = new DeleteChatRoomViewModel
                {
                    Id = chatRoom.Id,
                    ChatRoom = chatRoom,
                    //   SubCategories = subCats
                };

                return View(viewModel);
            }
        }

        public ActionResult DeleteChatRoom(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var chatRoom = _chatService.Get(id);
                    _chatService.Delete(chatRoom);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "ChatRoom Deleted",
                        MessageType = GenericMessages.success
                    };
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }

                return RedirectToAction("Index");
            }
        }
    }
}