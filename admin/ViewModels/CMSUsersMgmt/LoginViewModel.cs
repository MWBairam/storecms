using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.CMSUsersMgmt
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        //RememberMe in the Login.cshtml is a checkbox,
        //1-when it is checked, a persistent cookie is created when the user logs in.
        //2-if it is not checked, a session cookie is created when the user logs in.
        //for 1, if the user colses the browser without logout, then reopen it, the user will find himself still logged in.

        //for 2 no.
        //but for 2 we have a special case ! if we close the browser, and the RememberMe was not checked, then re opened the browser,  we might find ourselves logged in !!!
        //that because of the browser setting of "Continue with same previously opened tabs"
        //https://stackoverflow.com/questions/10617954/chrome-doesnt-delete-session-cookies 
        //so instartup file:
        //Also, set the Identity cookie expiration timeout for 1 hour so the user will have to login in again.
        //services.ConfigureApplicationCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(60));

    }
}
