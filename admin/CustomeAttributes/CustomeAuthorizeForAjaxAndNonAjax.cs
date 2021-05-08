using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.CustomeAttributes
{
    //inherit the Attribute class which allows us to use this class as an attribute [AjaxAttribute].
    //implement the IAsyncActionFilter to intercept the Http Request and check it.
    public class CustomeAuthorizeForAjaxAndNonAjax : Attribute, IAsyncActionFilter
    {
        //1-Properties:
        //Roles will contain the Roles string. e.g: "SuperAdmin, Admin, Guest".
        //we will split those later in below.
        public string Roles { get; set; }


        //2-Methods: (Interface Implementation)
        //implement the interface by implementing its method below:
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            //check the Role of the user if it matches any of the allowed specified Roles.
            //split the above Roles string to an array of roles.
            //check this array if the user's role matches any of its element.
            //once there is a match, assign isRoleBasedAuthorized=true and break the foreach loop.
            //because we need to prove that user's role matches only one of the roles array elements.
            //But first, check Roles if it is not empty, 
            //if it is not empty, that means we passed a value to Roles when we used this Attribute in a method (e.g: [CustomeAuthorizeForAjaxAndNonAjax(Roles ="SuperAdmin, Admin")] ) so process it.
            //if it is empty, that means we used this attribute without specifying Roles [CustomeAuthorizeForAjaxAndNonAjax], so consider the user is isRoleBasedAuthorized = true in the subsequesnt processing.
            bool isRoleBasedAuthorized = false;
            if (!string.IsNullOrEmpty(Roles))
            {
                string[] roles = Roles.Split(',');
                foreach (var role in roles)
                {
                    if (context.HttpContext.User.IsInRole(role))
                    {
                        isRoleBasedAuthorized = true;
                        break;
                    }
                    else
                    {
                        isRoleBasedAuthorized = false;
                    }
                }
            }
            else
            {
                 isRoleBasedAuthorized = true;
            }




            //check if the user is Authenticated (Signed In) using the User Identity which checks the AspNetCore Identity Cookie included in the request.
            //(remember that when the user logs in, a cookie is created and saved in the browser to hold his info)
            //And check the user's Roles.
            //Then allow to continue the http request to the method, or redirect him to login page:
            if (context.HttpContext.User.Identity.IsAuthenticated == false)
            {

                //check if the the request is an Ajax request (it has the word "XMLHttpRequest" in its header):
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var loginUrl = "/Account/Login";

                    //if not authenticated and the request is ajax, make the status code = 401 and save the redirect link in the result:
                    context.HttpContext.Response.StatusCode = 401;
                    //context.HttpContext.Response.SuppressFormsAuthenticationRedirect = true
                    JsonResult jsonResult1 = new JsonResult(new { redirectUrl = loginUrl });
                    context.Result = jsonResult1;
                    //then this will be intercepted in ajaxErrorHandler in wwwroot/js folder in order to redirect.

                    //return nothing !
                    return;
                }
                //if the request is not an ajax:
                else
                {
                    var loginUrl = $"/Account/Login";
                    //if the user is not authenticated and the request is not Ajax, simply redirect from here:
                    context.Result = new RedirectResult(loginUrl);

                    return;
                }
            }
            //if the user is authenticated, and Role-based authorized, simply pass the http request to the method:
            else if (context.HttpContext.User.Identity.IsAuthenticated == true && isRoleBasedAuthorized == true)
            {
                await next();
                return;
            }

            //the code reaches this area in 
            //the last case which is that user is authenticated, but his role does not match Roles.
            context.HttpContext.Response.StatusCode = 403;  //403 is Authenticated, but no permissions.
            var _errorMessage = " Authorized, you are not !";
            JsonResult jsonResult2 = new JsonResult(new { errorMessage = _errorMessage });
            context.Result = jsonResult2;
            //returns nothing:
            return;
        }
    }
}
