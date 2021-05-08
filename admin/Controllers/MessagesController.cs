using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using admin.Data;
using admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace admin.Controllers
{
    public class MessagesController : Controller
    {
        //1-Properties:
        private readonly ReversScaffoldedStoreContext _context;

        //2-Constructor:
        public MessagesController(ReversScaffoldedStoreContext context)
        {
            _context = context;
        }

        //3-Methods:
        // GET: Messages
        [Authorize] //Authorize for logged in users only, and without any role.
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.OrderByDescending(x => x.Id).ToListAsync());
        }

        // GET: Messages/ShowMessage/5
        public async Task<IActionResult> ShowMessage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

    }
}
