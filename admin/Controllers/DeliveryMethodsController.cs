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

namespace admin
{
    public class DeliveryMethodsController : Controller
    {
        //1-Properties:
        private readonly ReversScaffoldedStoreContext _context;

        //2-Constructor:
        public DeliveryMethodsController(ReversScaffoldedStoreContext context)
        {
            _context = context;
        }


        //3-Methods:
        // GET: DeliveryMethods
        public async Task<IActionResult> Index()
        {
            return View(await _context.DeliveryMethods.OrderByDescending(x=>x.Id).ToListAsync());
        }







        // GET: DeliveryMethods/AddOrEdit(Create)
        // GET: DeliveryMethods/AddOrEdit/5(Edit)
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            //in Index.cshtml, if the user clicked "Add"button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new DeliveryMethod());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the DeliveryMethod record of this id to be displayed and modified later.
                var Model = await _context.DeliveryMethods.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,ShortName,DeliveryTime,Description,Price")] DeliveryMethod Model)
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
                        if (!DeliveryMethodModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.DeliveryMethods.ToList()) });
                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool DeliveryMethodModelModelExists(int id)
        {
            return _context.DeliveryMethods.Any(e => e.Id == id);
        }











        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryMethod = await _context.DeliveryMethods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryMethod == null)
            {
                return NotFound();
            }

            return View(deliveryMethod);
        }

        // POST: DeliveryMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
            _context.DeliveryMethods.Remove(deliveryMethod);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:           
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.DeliveryMethods.ToList()) });
        }
    }
}
