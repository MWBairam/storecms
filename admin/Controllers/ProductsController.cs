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
using Microsoft.AspNetCore.Http;
using admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using admin.CustomeAttributes;

namespace admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ReversScaffoldedStoreContext _context;
        private readonly IUploadFile _upload;

        public ProductsController(ReversScaffoldedStoreContext context, IUploadFile upload)
        {
            _context = context;
            _upload = upload;
        }

        // GET: Products
        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> Index()
        {
            var reversScaffoldedStoreContext = _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType);
            return View(await reversScaffoldedStoreContext.OrderByDescending(x => x.Id).ToListAsync());
            //in Index.cshtml we are displaying the Brand Name and Type Name, not Id, so return the Brand and Type all info using .Include(p => p.ProductBrand).Include(p => p.ProductType)
        }







        // GET: Products/AddOrEdit(Create)
        // GET: Products/AddOrEdit/5(Edit)
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditProduct")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewData["ProductBrandId"] = new SelectList(_context.ProductBrands, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");

            //in Index.cshtml, if the user clicked "Add"button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new Product());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the Product record of this id to be displayed and modified later.
                var Model = await _context.Products.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditProduct")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Name,Description,Price,PictureUrl,ProductTypeId,ProductBrandId")] Product Model, IFormFile Picture)
        {

            if (ModelState.IsValid)
            {
                //Create
                if (id == 0)
                {
                    //upload a picture then save its path in the db table column :
                    //note that blogImg is a parameter above in the function brackets:
                    if (Picture != null && Picture.Length > 0)
                    {
                        //the method _upload saves the picture in a folder and returns its path.
                        string PictureUrl = await _upload.UploadFile(Picture, "images/products");
                        Model.PictureUrl = PictureUrl;
                    }


                    _context.Add(Model);
                    await _context.SaveChangesAsync();
                }
                //Update
                else
                {
                    try
                    {
                        //upload a picture then save its path in the db table column :
                        //note that blogImg is a parameter above in the function brackets:
                        if (Picture != null && Picture.Length > 0)
                        {
                            //the method _upload saves the picture in a folder and returns its path.
                            string PictureUrl = await _upload.UploadFile(Picture, "images/products");
                            Model.PictureUrl = PictureUrl;
                        }

                        _context.Update(Model);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToList()) });
                //in Index.cshtml we are displaying the Brand Name and Type Name, not Id, so return the Brand and Type all info using .Include(p => p.ProductBrand).Include(p => p.ProductType) 


                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool ProductModelModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }














        //No need for the Delete HttpGet method. We are sendeing Delete Post ajax request directly to here. 

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DeleteProduct")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:     
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToList()) });
            //in Index.cshtml we are displaying the Brand Name and Type Name, not Id, so return the Brand and Type all info using .Include(p => p.ProductBrand).Include(p => p.ProductType)
        }


    }
}
