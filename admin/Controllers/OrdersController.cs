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
    public class OrdersController : Controller
    {
        //1-Properties:
        private readonly ReversScaffoldedStoreContext _context;

        //2-Constructor:
        public OrdersController(ReversScaffoldedStoreContext context)
        {
            _context = context;
        }

        //3-Methods:
        // GET: Orders
        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> Index()
        {
            var reversScaffoldedStoreContext = _context.Orders.Include(o => o.DeliveryMethod);
            return View(await reversScaffoldedStoreContext.OrderByDescending(x => x.Id).ToListAsync());
        }


        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> ShowOrderItems(int id)
        {
            var orderItems = _context.OrderItems.Include(o => o.Order).Where(m => m.OrderId == id);
            if (orderItems == null)
            {
                return NotFound();
            };
            return View(await orderItems.OrderByDescending(x => x.Id).ToListAsync());
        }





        // GET: Orders/AddOrEdit(Create)
        // GET: Orders/AddOrEdit/5(Edit)
        [NoDirectAccess] //this attribute from the Helpers folder we created, so the user is prohibited from accessing /<ControllerName>/AddOrEdit directly, and allowed only through ajax request.
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditOrder")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewData["DeliveryMethodId"] = new SelectList(_context.DeliveryMethods, "Id", "ShortName");

            //in Index.cshtml, if the user clicked "Add"button, no id will be sent, and id will be 0 as it is the default value above.
            if (id == 0)
            {
                //the user wants to create a record, so return an empty model to be dislayed and filled.
                return View(new Order());
            }
            else
            {
                //else, that means the user clicked on Edit, and there is a value for the id other than 0,
                //so return the Product record of this id to be displayed and modified later.
                var Model = await _context.Orders.FindAsync(id);
                if (Model == null)
                {
                    return NotFound();
                }
                return View(Model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "AddOrEditOrder")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,BuyerEmail,OrderDate,ShipToAddressFirstName,ShipToAddressLastName,ShipToAddressStreet,ShipToAddressCity,ShipToAddressState,ShipToAddressZipcode,DeliveryMethodId,Subtotal,Status,PaymentIntentId")] Order Model)
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
                        if (!OrderModelModelExists(Model.Id))
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
                return Json(new { isValid = true, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Orders.Include(p => p.DeliveryMethod).ToList()) });
                //in Index.cshtml we are displaying the DeliveryMethod ShortName, not Id, so return the DeliveryMethod all info using .Include(p => p.DeliveryMethod)


                //return NotFound();
            }
            //if the model submitted is not valid according to the Attriburtes in [] in the model file in Models folder:
            return Json(new { isValid = false, html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "AddOrEdit", Model) });
        }
        private bool OrderModelModelExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }































        //No need for Delete HttpGet method, we are calling the below post method with ajax request directly.

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomeAuthorizeForAjaxAndNonAjax(Roles = "DeleteOrder")] //This method is called using ajax requests so authorize it with the custome attribute we created for the logged in users with the appropriate role.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            //After the successfull delete, we do not return a view because we already did the Create or Edit using Ajax request,
            //which means we did not reload the page, so return the _ViewAll.cshtml which has the html table, return it as serialized html in json file, to be rendered in Index.cshtml as a partial view:     
            return Json(new { html = SerializeHtmlElemtnsToString.RenderRazorViewToString(this, "_ViewAll", _context.Orders.Include(p => p.DeliveryMethod).ToList()) });
            //in Index.cshtml we are displaying the DeliveryMethod ShortName, not Id, so return the DeliveryMethod all info using .Include(p => p.DeliveryMethod)
        }
    }
}
