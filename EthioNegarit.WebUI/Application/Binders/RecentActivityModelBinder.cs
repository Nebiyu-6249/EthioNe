using System.Web.Mvc;
using EthioNegarit.Core.Domain;
namespace EthioNegarit.WebUI.Application.Binders
{
    public class RecentActivityModelBinder: IModelBinder
    {
        private const string sessionKey = "RecentActivity";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Get the  RecentActivity from the session
            RecentActivity recentActivity = null;
            if (controllerContext.HttpContext.Session != null)
            {
                recentActivity = (RecentActivity)controllerContext.HttpContext.Session[sessionKey];
            }
            // Create RecentActivity if there wasn't one in the session data
            if (recentActivity == null)
            {
                recentActivity = new RecentActivity();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = recentActivity;
                }
            }
            return recentActivity;
        }
    }
}