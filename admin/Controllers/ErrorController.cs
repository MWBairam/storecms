using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    public class ErrorController : Controller
    {
        //1-Properties:
        //The IHostEnvironment allows us to read the /Properties/lauchSettings.json, ASPNETCORE_ENVIRONMENT properties there,
        //in order to determine if we are in the in the Development mode or in Production mode.
        public readonly IHostEnvironment _env;
        private readonly ILogger _logger;

        //2-Constructor:
        //Dependency injection for the IHostEnvironment service:
        public ErrorController(IHostEnvironment env, ILogger<ErrorController> logger)
        {
            _env = env;
            _logger = logger;
        }

        /*
        {statusCode} will receive the http status error code passed to this method when we passed it in the startup file in
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        */
        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

                    if (_env.IsDevelopment())
                    {
                        //if we are in the Development mode, create thos temporary data variables,
                        //so that NotFound.cshtml view will read of these and display them there:

                        ViewBag.ErrorMessage = $"Error {statusCode} NotFound !";
                        ViewBag.Path = $"Requested Path: {statusCodeResult.OriginalPath}";
                        ViewBag.QueryString = $"Query Parameters: {statusCodeResult.OriginalQueryString}";
                    }
                    else
                    {
                        //if we are in the Production mode, only create this temp data variable, which holds the error code,
                        //sp that NotFound.cshtml will read this only and display it to the user, and no nedd to display anything else.:
                        ViewBag.ErrorMessage = $"Error {statusCode} !";
                    }

                    //log the exception:
                    //remember that we have configured the application to log to console/debug visual studio window and to file.
                    //logging config written in program.cs, NLog provider config file is nlog.config.
                    _logger.LogWarning($"{statusCode} Error Occured. Path = {statusCodeResult.OriginalPath} adn QueryString = {statusCodeResult.OriginalQueryString}");

                    break;

                    /*
                    no need to handle other cases of:

                    case 400: (BadRequest)
                    case 401: (UnAuthorized Request)

                    because we will not have these http errors reach here !
                    why ?
                    that is because in our project, we use Ajax requests (remember in Index.cshtml, the add, edit and delete buttons call
                    Ajax methods in wwwroot/js/site.js, and Ajax methods does not reload the browser with a new request, it opens a hidden pipeline
                    and sends the request there, and receives the http error in that hidden pipeline.
                    So to handle them, we created the wwwroot/js/ajaxErrorHandler.
                    It is good to note that error 404 can happen as well in ajax requests.
                    */
                    /*
                    Also, we do not handle the http error 500 (internal server error) here, because it happens when an asp.net code throws
                    an exception ! and we will handle it below in another method.
                    */
            }

            //The return the NotFound view to display the above TempData:
            return View("NotFound");
        }


        [Route("Exception")]
        [AllowAnonymous]
        public IActionResult ExceptionHandler()
        {

            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (_env.IsDevelopment())
            {
                //when we are in development mode, create those temp data to be displaed in Exception.cshtml,
                //which are useful for a developer:
                
                ViewBag.ExceptionPath = exceptionDetails.Path;
                ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
                ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;
            }
            else
            {   //when we are in the Production mode, do not display data to the user, and display this message only:
                ViewBag.ExceptionPath = "your reuested path !";
                ViewBag.ExceptionMessage = "Server Internal Error ! Please Try again later.";
                ViewBag.Stacktrace = "Stack has gone away !";
            }

            //log the exception:
            //remember that we have configured the application to log to console/debug visual studio window and to file.
            //logging config written in program.cs, NLog provider config file is nlog.config.
            _logger.LogError($"The Path {exceptionDetails.Path} threw an exception: {exceptionDetails.Error}");

            return View("Exception");
        }
    }
}
