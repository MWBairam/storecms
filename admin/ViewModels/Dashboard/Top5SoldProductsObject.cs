using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class Top5SoldProductsObject
    {
        //those should match the .select query in DashboardController, var _Top5SoldProducts.
        //This will be used as List<> object type in DashboardViewModel.
        public string ProductName { get; set; }
        public int Count { get; set; }
    }
}
