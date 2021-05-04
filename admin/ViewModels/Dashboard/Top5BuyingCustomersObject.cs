using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class Top5BuyingCustomersObject
    {
        //those should match the .select query in DashboardController, var _Top5BuyingCustomers
        //This will be used as List<> object type in DashboardViewModel.
        public string CustomerName { get; set; }
        public decimal AmountPaidSum { get; set; }
    }
}
