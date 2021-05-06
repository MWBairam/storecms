﻿using admin.Models.CmsIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using admin.ViewModels.CMSUsersMgmt;
using admin.Helpers;

namespace admin.Controllers
{
    public class AccountProfileController : Controller
    {
        //1-properties:
        private readonly UserManager<CmsUser> _userManager; //instead if the default IdentityUser, use the CmsUser class I created.
        private readonly SignInManager<CmsUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //2-constructor
        public AccountProfileController(UserManager<CmsUser> userManager, SignInManager<CmsUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var SignedInUserId = _userManager.GetUserId(HttpContext.User);
                var SignedInUser = _userManager.FindByIdAsync(SignedInUserId).Result;
                if (SignedInUser == null)
                {
                    return NotFound();
                }
                var AccountProfileViewModel = new AccountProfileViewModel
                {
                    Id = SignedInUser.Id,
                    DisplayName = SignedInUser.DisplayName,
                    UserName = SignedInUser.UserName,
                    Email = SignedInUser.Email
                };
                return View(AccountProfileViewModel);
            }
            return NotFound();

        }

        #region EditMyProfile
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/EditUser directly, and allowed only through ajax request.
        public async Task<IActionResult> EditMyProfile()
        {
            //bring the current logged in user's info using the GetUserAsync(User) whereas "User" is a predefined parameter in microsoft identity to represent the logged in user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //return RedirectToAction("Login", "Account");
                //we are calling this method using ajax request, so it is hard to redirect without specific modifications, so:
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
        public async Task<IActionResult> EditMyProfile(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //bring the current logged in user's info using the GetUserAsync(User) whereas "User" is a predefined parameter in microsoft identity to represent the logged in user.
                var user = await _userManager.GetUserAsync(User);
                //or we can use the model.Email for that

                if (user == null)
                {
                    //return RedirectToAction("Login", "Account");
                    //we are calling this method using ajax request, so it is hard to redirect without specific modifications, so:
                    return NotFound();
                }
                user.DisplayName = model.DisplayName;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    //After the successfull Create or Edit, we do not return a view because we already did the Create or Edit using Ajax request,
                    //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:

                    var AccountProfileViewModel = new AccountProfileViewModel
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        Email = user.Email
                    };

                    return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", AccountProfileViewModel) });
                    //return NotFound();
                }

                // If result is not successful, If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditMyProfile", model) });
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "EditMyProfile", model) });

        }
        #endregion


        #region ChangeMyPassword
        [HttpGet]
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/EditUser directly, and allowed only through ajax request.
        public async Task<IActionResult> ChangeMyPassword()
        {
            //bring the current logged in user's info using the GetUserAsync(User) whereas "User" is a predefined parameter in microsoft identity to represent the logged in user.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //return RedirectToAction("Login", "Account");
                //we are calling this method using ajax request, so it is hard to redirect without specific modifications, so:
                return NotFound();
            }
            var ChangeMyPasswordViewModel = new ChangeMyPasswordViewModel
            {
                CurrentPassword = "",
                NewPassword = "",
                ConfirmPassword = ""
            };
            return View(ChangeMyPasswordViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeMyPassword(ChangeMyPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //bring the current logged in user's info using the GetUserAsync(User) whereas "User" is a predefined parameter in microsoft identity to represent the logged in user.
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    //return RedirectToAction("Login", "Account");
                    //we are calling this method using ajax request, so it is hard to redirect without specific modifications, so:
                    return NotFound();
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user,model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                    return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "ChangeMyPassword", model) });

                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                var AccountProfileViewModel = new AccountProfileViewModel
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Email = user.Email
                };
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", AccountProfileViewModel) });

            }

            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "ChangeMyPassword", model) });

        }
        #endregion

    }
}
