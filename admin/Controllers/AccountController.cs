using admin.CustomeAttributes;
using admin.Data;
using admin.Helpers;
using admin.Models.CmsIdentity;
using admin.ViewModels.CMSUsersMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    public class AccountController : Controller
    {
        //1-properties:
        private readonly UserManager<CmsUser> _userManager; //instead if the default IdentityUser, use the CmsUser class I created.
        private readonly SignInManager<CmsUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //2-constructor
        public AccountController(UserManager<CmsUser> userManager, SignInManager<CmsUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        //3-Methods:
        [Authorize] //Authorize for logged in users only, and without any role.
        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users);
        }





        #region Register
        //Get Register View by requesting ..../account/register:
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/Register directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "RegisterNewCMSUser")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public IActionResult Register()
        {
            return View();
        }
        //post register data written in register form in Register.cshtml:
        [HttpPost]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "RegisterNewCMSUser")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Create new user object as ApplicationUser instance, thensubmit it to AspNetUsers using UserManager.
                var user = new CmsUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.DisplayName
                    //AspNetUsers table has nearly 14 columns, we fill email and and password only.
                    //others will be filled by microsoft if requierd.
                    //Also, we added the column DisplayName, so we will fill it as well.

                };
                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);
                //this will create a user, and assign true to "result" if creation is successful.
                //if creation is not successful, "result" will contain Errors array and description for each one.

                // If user is successfully created, sign-in the user using:


                if (result.Succeeded)
                {
                    //After the successfull Create or Edit, we do not return a view because we already did the Create or Edit using Ajax request,
                    //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
                    return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _userManager.Users) });
                    //return NotFound();
                }

                //If result is not success, If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "Register", model) });
            }

            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "Register", model) });
        }
        //The below method is used for the Remote-Validation process to check if an email (username) is used by an already existed user before submiting the register data to DB.
        //for that, we added the validation attribute  [Remote(action: "IsEmailInUse", controller: "Account")] in the RegisterViewModel.
        //this method will respond to HttpGet and HttpPost calls, so that we can write:
        //[HttpGet][HttpPost]
        //or use the AcceptVerbs attribute.
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }
        #endregion


        #region Edituser
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/EditUser directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "EditCMSUser")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> EditUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound();
            }

            var EditUserViewModel = new EditUserViewModel
            {
                Email = user.Email,
                DisplayName = user.DisplayName
            };
            return View(EditUserViewModel);
        }
        [HttpPost]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "EditCMSUser")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if nothing was found:
                if (user == null)
                {
                    return NotFound();
                }
                user.DisplayName = model.DisplayName;
                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    //After the successfull Create or Edit, we do not return a view because we already did the Create or Edit using Ajax request,
                    //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
                    return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _userManager.Users) });
                    //return NotFound();
                }

                // If the result is not success, If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditUser", model) });
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditUser", model) });

        }
        #endregion


        #region DeleteUser
        //No need for the Delete HttpGet method. We are sendeing Delete Post ajax request directly to here. 

        // POST: Account/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DeleteCMSUser")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if(result.Succeeded)
            {
                //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
                //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:           
                return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _userManager.Users) });

            }
            //else
            return BadRequest();
        }
        #endregion


        #region ResetPasswordByAdmin
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/EditUser directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "ResetCMSUserPassword")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> ResetPasswordByAdmin(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound();
            }
            var ResetPasswordByAdmin = new ResetPasswordByAdmin
            {
                Email = user.Email,
                Password = "",
                ConfirmPassword = ""
            };
            return View(ResetPasswordByAdmin);
        }
        [HttpPost]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "ResetCMSUserPassword")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> ResetPasswordByAdmin(ResetPasswordByAdmin model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if nothing was found:
                if (user == null)
                {
                    return NotFound();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (result.Succeeded)
                {
                    //After the successfull Create or Edit, we do not return a view because we already did the Create or Edit using Ajax request,
                    //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
                    return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _userManager.Users) });
                    //return NotFound();
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "ResetPasswordByAdmin", model) });
        }
        #endregion


        #region Login
        //login methods are not called using ajax methods.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if the model sent from the LoginView form is valid:
                //sign in the user using his password:
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home"); 
                }
                else
                {
                    //else, display this error:
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }

            //if the model is not valid, return the view with validation errors:
            return View(model);
        }
        #endregion


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        #region EditUserRole
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/EditUser directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "EditUserRoles")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> EditUserRoles(string _Email)
        {
            /*
            Note:
            -We are executing 3 Select (Get or Read) calls to the DB in:
            var users _userManager.FindByEmail(_Email);
            var AllRoles = _roleManager.Roles;
            await _userManager.IsInRoleAsync(user, role.Name)
            
            -Unfortunately, the PostgresDB does not support multiple select calls for one instance of DbContext,
            and we wil get an exception thrown saying: "A command is already in progress" when performing the second one !
            
            -The solution was found bu using .ToList() in the first and second command.
            ToList() forced the aoolication to stop and wat untill the execution is finished, then moved to the execute the second one.
            */

            //Bring the user, but instead of _userManager.FindByEmail(_Email), use the below as we explained in the note above:
            var user = _userManager.Users.Where(p => p.Id != null).ToList().Where(x => x.Email == _Email).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            //Bring the all roles we have, but instead of _roleManager.Roles, use the below as we explained in the note above:
            var AllRoles = _roleManager.Roles.Where(p => p.Id != null).ToList();

            var UserRolesList = new List<UserRoleViewModel>();
            foreach (var role in AllRoles)
            {
                var UserRoleViewModel = new UserRoleViewModel
                {
                    userEmail = user.Email,
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                //if the user has this rule, mark IsSelected with true, else mark it with false:
                UserRoleViewModel.IsSelected = await _userManager.IsInRoleAsync(user, role.Name) == true ? true : false;

                UserRolesList.Add(UserRoleViewModel);
            }


            return View(UserRolesList);
        }

        [HttpPost]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "EditUserRoles")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> EditUserRoles(List<UserRoleViewModel> model)
        {
            /*
            Note:
            -We are executing 2 Select (Get or Read) calls to the DB in:
            var users _userManager.FindByEmail(_Email);
            var roles = await _userManager.GetRolesAsync(user);
            
            -Unfortunately, the PostgresDB does not support multiple select calls for one instance of DbContext,
            and we wil get an exception thrown saying: "A command is already in progress" when performing the second one !
            
            -The solution was found bu using .ToList() in the first command.
            ToList() forced the aoolication to stop and wat untill the execution is finished, then moved to the execute the second one.

            -The other commands: 
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            are not select commands, so no problem with them.
            */




            //We need to bring the user info who we are going to modify his roles,
            //we will bring him by his email,
            //the user's email is existed in each object "UserRoleViewModel" in the list "List<UserRoleViewModel>",
            //so we can take the email from the object of index=0 for exampl,
            //but instead of executing var users _userManager.FindByEmail(_Email), we did the below according to the note above:
            var user = _userManager.Users.Where(p => p.Id != null).ToList().Where(x => x.Email == model[0].userEmail).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            //bring all the Roles which are assigned to this user:
            var roles = await _userManager.GetRolesAsync(user);
            //Remove all the assigned Roles to the user we have, so we can add what we want below:
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                //in case of errors:
                return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditUserRoles", model) });
            }
            //Now re-assign new Roles to the user according to the IsSelected checkbox of the UserRoleViewModel objects:
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                //in case of errors:
                return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditUserRoles", model) });
            }

            //After the successfull Edit, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
            return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _userManager.Users) });

        }
        #endregion


        //this method will be called once a user in not authorized to call an authorized method with [Authorize] attribute.
        //For the [CustomeAuthorizeForAjaxAndNonAjax] we show a toastr notification. Please read the  services.ConfigureApplicationCookie in startup file.
        //asp will redirect the user to /Account/AccessDenied here.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            //it returns the view: /Views/Account/AccessDenied:
            return View();
        }















    }
}
