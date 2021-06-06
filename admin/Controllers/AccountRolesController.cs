using admin.Helpers;
using admin.Models.CmsIdentity;
using admin.ViewModels.CMSUsersMgmt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using admin.CustomeAttributes;

namespace admin.Controllers
{
    public class AccountRolesController : Controller
    {
        //1-Properties:
        //RoleManager to create/edit/delete roles in AspNetRoles table.
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CmsUser> _userManager;

        //2-Constructor:
        //inject the RoleManager:
        public AccountRolesController(RoleManager<IdentityRole> roleManager, UserManager<CmsUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        //3-methods:
        [Authorize] //Authorize for logged in users only, and without any role.
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }





        ////Create Role:
        //[HttpGet]
        //[CustomeAuthorizeForAjaxAndNonAjax(Roles = "CreateRole")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        //public IActionResult CreateRole()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // We just need to specify a unique role name to create a new role
        //        IdentityRole identityRole = new IdentityRole
        //        {
        //            Name = model.RoleName
        //        };

        //        // Saves the role in the underlying AspNetRoles table
        //        IdentityResult result = await _roleManager.CreateAsync(identityRole);

        //        if (result.Succeeded)
        //        {
        //            //After the successfull Create, we do not return a view because we already did the Create using Ajax request,
        //            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
        //            return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _roleManager.Roles.ToList()) });

        //        }

        //        foreach (IdentityError error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }

        //        //if there was an error creating the role :
        //        return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "CreateRole", model) });
        //    }
        //    //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
        //    return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "CreateRole", model) });
        //}






        //DisplayRoleInfo:
        // Role ID is passed from the URL to the action
        [HttpGet]
        [NoDirectAccessAttribute]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DisplayRoleInfo")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DisplayRoleInfo(string id)
        {
            // Find the role by Role ID
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var model = new DisplayRoleInfoViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            /*
            Note:
            -We are executing 2 Select calls to the DB in:
            var Users = _userManager.Users
            await _userManager.IsInRoleAsync(user, role.Name)
            
            -Unfortunately, the PostgresDB does not support multiple select calls for one instance of DbContext,
            and we wil get an exception thrown saying: "A command is already in progress" when performing the second one !
            
            -The solution was found bu using .ToList() in the first command.
            ToList() forced the aoolication to stop and wat untill the execution is finished, then moved to the execute the second one.
            */
            var Users = _userManager.Users.Where(p => p.Id != null).ToList();

            foreach (var user in Users)
            {
                // If the user is in this role, add the username to
                // Users property of DisplayRoleInfoViewModel. This model
                // object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
                //Roles are in AspNetUsers
                //Users are in AspNetRoles
                //Roles granted to each user are in AspNetUsersRoles
                //IsInRoleAsync checks the AspNetUsersRoles to check the roles of each user.
            }

            //now return the Edit.cshtml with passing to it DisplayRoleInfoViewModel which has the Role Id, name, list of associated users:
            return View(model);
        }

     





        //No DeleteRole method as well, because we consider roles are hard coded.



    }
}
