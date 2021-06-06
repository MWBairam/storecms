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
using Microsoft.AspNetCore.Authorization;
using admin.CustomeAttributes;

namespace admin.Controllers
{
    public class CustomersAddressesController : Controller
    {
        private readonly ReversScaffoldedStoreIdentityContext _context;

        public CustomersAddressesController(ReversScaffoldedStoreIdentityContext context)
        {
            _context = context;
        }

        // GET: CustomersAddresses
        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> Index()
        {
            var reversScaffoldedStoreIdentityContext = _context.Addresses.Include(a => a.AppUser);
            return View(await reversScaffoldedStoreIdentityContext.ToListAsync());
        }







        // GET: CustomersAddresses/AddOrEdit(Create)
        // GET: CustomersAddresses/AddOrEdit/5(Edit)
        [NoDirectAccessAttribute] //this attribute from the CustomeAttributes folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditCustomerAddress")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewData["AppUserId"] = new SelectList(_context.AspNetUsers, "Id", "Email");

            //in Index.cshtml, if the user clicked "Add" button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new Address());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the ProductBrand record of this id to be displayed and modified later.
                var Model = await _context.Addresses.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditCustomerAddress")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,FirstName,LastName,Street,City,State,Zipcode,AppUserId")] Address Model)
        {
            if (ModelState.IsValid)
            {
                //Create
                if (id == 0)
                {
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
                        if (!CustomerAddressModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Addresses.Include(a => a.AppUser).ToList()) });
                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool CustomerAddressModelModelExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }























        //No need for the Delee HttpGet method, we are calling the below post method using ajax request directly.

        // POST: CustomersAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DeleteCustomerAddress")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:           
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Addresses.Include(a => a.AppUser).ToList()) });

        }

    }
}
