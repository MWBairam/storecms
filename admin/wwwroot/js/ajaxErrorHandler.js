//Show and log a message when an Ajax request fails.
//we are using the event .ajaxError
//Refrences:
//https://api.jquery.com/ajaxerror/
//https://www.w3schools.com/jquery/ajax_ajaxerror.asp

$(document).ajaxError(function (event, jqxhr, settings, thrownError) {

    if (jqxhr.status == 401) {
        //-For the methods protected with [CustomeAuthorizeForAjaxAndNonAjax] (methods calle by an ajax request),
        //What if the user is not logged in somhow (or identity cookie is deleted) ?
        //-For this case, in the[CustomeAuthorizeForAjaxAndNonAjax], I wrote the code to return http error 401.
        //- In wwwroot / js / ajaxErrorHandler.js here, it catches the http error 401 and redirects the user to / Account / Login.
        window.location.href = jqxhr.responseJSON.redirectUrl;
    }
    else if (jqxhr.status == 403) {
        //-But what if the user is logged in, but does not have the appropriate role ?
        //-For this case, in the[CustomeAuthorizeForAjaxAndNonAjax], I wrote the code to return http error 403.
        //- In wwwroot / js / ajaxErrorHandler.js here, it catches the http error 403 and displayes a toastr notifiction.
        //the message is in the returned http error as json file new JsonResult(new { errorMessage = "Authorized, you are not !" });
        toastr.error('Error ' + jqxhr.status + ' !' + jqxhr.responseJSON.errorMessage);
    }
    else if (jqxhr.status == 404) {
        toastr.error('Error ' + jqxhr.status + ' !' + 'Not Found !');
    }
    else if (jqxhr.status == 400) {
        toastr.error('Error ' + jqxhr.status + ' !' + 'A bad request, you have made !');
    }
    else if (jqxhr.status == 500 || jqxhr.status == 503) {
        toastr.error('Error ' + jqxhr.status + ' !' + 'Internal Server Error !');
        console.log('Error requesting page ' + settings.url + '. ' + 'Error ' + jqxhr.status + ' of ' + thrownError + ' was returned.');
    }
    else {
        toastr.error('Error ' + jqxhr.status + ' !');
        console.log('Error requesting page ' + settings.url + '. ' + 'Error ' + jqxhr.status + ' of ' + thrownError + ' was returned.');
    }
});














//If the request failed because JavaScript raised an exception, the exception object is passed to the handler as a fourth parameter thrownError.

/*
We can check if the jqxhr.status error number is one of the folloiwng: 400, 401, 403, 404, 500, 503 .....
then display a custome message for that:
                '400' : "Server understood the request, but request content was invalid.",
                '401' : "Unauthorized access.",
                '403' : "Forbidden resource can't be accessed.",
                '500' : "Internal server error.",
                '503' : "Service unavailable."
Examples: https://stackoverflow.com/questions/377644/jquery-ajax-error-handling-show-custom-exception-messages
*/

/*
For example, we can restrict our callback to only handling events dealing with a particular URL:

$('.log').ajaxError(function(e, xhr, settings, exception) {
  if (settings.url == 'ajax/missing.html') {
    $(this).text('Triggered ajaxError handler.');
  }
});
 */