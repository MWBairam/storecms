using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using admin.Data;
using admin.Models.StoreIdentityModels;
using admin.Helpers;
using Microsoft.AspNetCore.Identity;

namespace admin.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ReversScaffoldedStoreIdentityContext _context;


        public CustomersController(ReversScaffoldedStoreIdentityContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AspNetUsers.ToListAsync());
        }









        // GET: Customer/AddOrEdit(Create)
        // GET: Customer/AddOrEdit/5(Edit)
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        public async Task<IActionResult> AddOrEdit(string id = "")
        {
            //in Index.cshtml, if the user clicked "Add" button, no id will be sent, and id will be null string as it is the default value above.
            if (string.IsNullOrEmpty(id))
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new AspNetUser());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than null,
                //so return the Customer record of this id to be displayed and modified later.
                var Model = await _context.AspNetUsers.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, [Bind("Id,DisplayName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]  AspNetUser Model)
        {
            if (ModelState.IsValid)
            {
                //Create
                if (string.IsNullOrEmpty(id))
                {
                    //This creation in "AspNetUsers" table (reversly scaffolded from "skinet" application tables) is wrong.
                    //The creation should be with UserManager Service from Microsoft identity library.
                    //Anyway, we are not allowing the creation to be used here, but I kept this anyway.
                    _context.Add(Model);
                    await _context.SaveChangesAsync();
                }
                //Update
                else
                {
                    try
                    {
                        _context.Update(Model);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CustomerExists(Model.Email))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                //After the successfull Create or Edit, we do not return a view because we already did the Create or Edit using Ajax request,
                //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.AspNetUsers.ToList()) });
                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool CustomerExists(string email)
        {
            return _context.AspNetUsers.Any(e => e.Email == email);
        }






        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            _context.AspNetUsers.Remove(aspNetUser);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:           
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.AspNetUsers.ToList()) });

        }

    }
}
