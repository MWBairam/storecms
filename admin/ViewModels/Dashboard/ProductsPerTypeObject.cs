using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class ProductsPerTypeObject
    {
        //those should match the .select query in DashboardController, var _ProductsPerType
        //This will be used as List<> object type in DashboardViewModel.
        public string TypeName { get; set; }
        public int Count { get; set; }
    }
}
