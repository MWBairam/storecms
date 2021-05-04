using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class DashboardViewModel
    {


        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSoldItems { get; set; }
        public int TotalBrands { get; set; }
        public int TotalTypes { get; set; }
        public List<ProductsPerBrandObject> ProductsPerBrand { get; set; }
        public List<ProductsPerTypeObject> ProductsPerType { get; set; }
        public List<Top5SoldProductsObject> Top5SoldProducts { get; set; }
        public List<Top5BuyingCustomersObject> Top5BuyingCustomers { get; set; }
        public List<Last30DaysWhenOrdersTookPlaceObject> Last30DaysWhenOrdersTookPlace { get; set; }
        public decimal TotalOf_Last30DaysWhenOrdersTookPlaceList { get; set; }

        public DashboardViewModel()
        {
            //initialize the lists as new lists once instantiated in the DashboardController:
            ProductsPerBrand = new List<ProductsPerBrandObject>();
            ProductsPerType = new List<ProductsPerTypeObject>();
            Top5SoldProducts = new List<Top5SoldProductsObject>();
            Top5BuyingCustomers = new List<Top5BuyingCustomersObject>();
            Last30DaysWhenOrdersTookPlace = new List<Last30DaysWhenOrdersTookPlaceObject>();
        }
    }
}
