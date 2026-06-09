using System.Web.Mvc;
using EthioNegarit.Core;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.WebUI.Application;
using EthioNegarit.WebUI.Areas.Admin.ViewModels;
using EthioNegarit.Utilities;

namespace EthioNegarit.WebUI.Areas.Admin.Controllers
{
    public class AdminController : BaseAdminController
    {
        
          public AdminController(ILoggingService loggingService,
              IUnitOfWorkManager unitOfWorkManager,
              IMembershipService membershipService,
              ILocalizationService localizationService,
              ISettingsService settingsService)
              : base(loggingService, unitOfWorkManager, membershipService, localizationService, settingsService)
          {
          }

          /// <summary>
          /// Default page for the admin area
          /// </summary>
          /// <returns></returns>
          public ActionResult Index()
          {
              return View();
          }

     /*
        #region Utils
          [HttpPost]
          public void Aptitude()
          {
              try
              {
                  var url = StringUtils.ReturnCurrentDomain();
                  var postString = string.Concat("url=", url);
                  var response = StringUtils.PostForm("http://www.negarit.com/base/MVCBase/DomainCheck", postString);
              }
              catch
              {
                  // No need to do anything
              }
          } 
          #endregion

        */
    }
}