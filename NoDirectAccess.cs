using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Policy;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GeneralCargoSystem.Utility
{
    public class NoDirectAccess : ActionFilterAttribute
    {
        public override void  OnActionExecuting(ActionExecutingContext filterContext)
        {
            var canAccess = false;

            // check the refer
            var referer = filterContext.HttpContext.Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer))
            {
                var rUri = new UriBuilder(referer).Uri;
                var req = filterContext.HttpContext.Request;
                if (req.Host.Host == rUri.Host /*&& req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme*/)
                {
                    canAccess = true;
                }
            }
            if (!canAccess)
            {


                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Dashboard", action = "Index", area = "GCAnalytics" }));
            }
        }
    }
}
