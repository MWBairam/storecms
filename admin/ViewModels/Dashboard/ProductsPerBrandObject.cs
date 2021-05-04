using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class ProductsPerBrandObject
    {
        //those should match the .select query in DashboardController, var _ProductsPerBrand
        //This will be used as List<> object type in DashboardViewModel.
        public string BrandName { get; set; }
        public int Count { get; set; }
    }
}
