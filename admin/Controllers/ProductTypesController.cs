using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using admin.Data;
using admin.Models;
using admin.Helpers;
using Microsoft.AspNetCore.Authorization;
using admin.CustomeAttributes;

namespace admin.Controllers
{
    public class ProductTypesController : Controller
    {
        private readonly ReversScaffoldedStoreContext _context;

        public ProductTypesController(ReversScaffoldedStoreContext context)
        {
            _context = context;
        }

        // GET: ProductTypes
        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductTypes.OrderByDescending(x => x.Id).ToListAsync());
        }





        // GET: ProductsTypes/AddOrEdit(Create)
        // GET: ProductsTypes/AddOrEdit/5(Edit)
        [NoDirectAccessAttribute] //this attribute from the CustomeAttributes folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditProductType")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            //in Index.cshtml, if the user clicked "Add"button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new ProductType());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the ProductType record of this id to be displayed and modified later.
                var Model = await _context.ProductTypes.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditProductType")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Name")] ProductType Model)
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
                        if (!ProductTypeModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.ProductTypes.ToList()) });
                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool ProductTypeModelModelExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }







        //No need for the Delete HttpGet method. We are sendeing Delete Post ajax request directly to here. 

        // POST: ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DeleteProductType")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:          
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.ProductTypes.ToList()) });

        }

    }
}
