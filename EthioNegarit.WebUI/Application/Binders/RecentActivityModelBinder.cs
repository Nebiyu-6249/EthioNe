using EthioNegarit.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace EthioNegarit.WebUI.Application.Binders
{
    public class RecentActivityModelBinder: IModelBinder
    {
        private const string sessionKey = "RecentActivity";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // get the Cart from the session
            RecentVisited recentVisited = null;
            if (controllerContext.HttpContext.Session != null)
            {
                recentVisited = (RecentVisited)controllerContext.HttpContext.Session[sessionKey];
            }
            // create the Cart if there wasn't one in the session data
            if (recentVisited == null)
            {
                recentVisited = new RecentVisited();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = recentVisited;
                }
            }
            // return the cart
            return recentVisited;
        }
    }
}