using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        //We are calling "Create" and "Edit" methods in controllers using Ajax requests.
        //Remember that each method is 2 parts, HttpGet to return the Create.cshtml and Edit.cshtml, and the HttpPost which receives the submitted data.
        //We are displaying the Create.cshtml and Edit.cshtml in bootstrap modal popup without reloading the current view.
        //So we We should prevent the user from calling those methods by typing a url like https://localhost:port/Create and reload the view.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.GetTypedHeaders().Referer == null
                ||
            filterContext.HttpContext.Request.GetTypedHeaders().Host.Host.ToString() != filterContext.HttpContext.Request.GetTypedHeaders().Referer.Host.ToString())
            {
                filterContext.HttpContext.Response.Redirect("/");
            }
        }



    }
}
