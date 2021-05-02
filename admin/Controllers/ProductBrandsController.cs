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

namespace admin.Controllers
{
    public class ProductBrandsController : Controller
    {
        //1-Properties:
        //our DbContext:
        private readonly ReversScaffoldedStoreContext _context;


        //2-Constructor:
        //inject the DbContext:
        public ProductBrandsController(ReversScaffoldedStoreContext context)
        {
            _context = context;
        }



        // GET: ProductBrands
        public async Task<IActionResult> Index()
        {
            //return the entire list to the view, then in the view we will use bootstrap datatable for pagination, sorting and search.
            //by default, list is returned orderd by Id ascending:
            return View(await _context.ProductBrands.ToListAsync());
        }





        // GET: ProductsBrands/AddOrEdit(Create)
        // GET: ProductsBrands/AddOrEdit/5(Edit)
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            //in Index.cshtml, if the user clicked "Add"button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new ProductBrand());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the ProductBrand record of this id to be displayed and modified later.
                var Model = await _context.ProductBrands.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Name")] ProductBrand Model)
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
                        if (!ProductBrandModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.ProductBrands.ToList()) });
                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool ProductBrandModelModelExists(int id)
        {
            return _context.ProductBrands.Any(e => e.Id == id);
        }













        //No need for the Delete HttpGet method. We are sendeing Delete Post ajax request directly to here. 

        // POST: ProductBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productBrand = await _context.ProductBrands.FindAsync(id);
            _context.ProductBrands.Remove(productBrand);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:           
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.ProductBrands.ToList()) });
        }


    }
}
