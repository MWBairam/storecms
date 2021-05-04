using admin.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using admin.ViewModels.Dashboard;

namespace admin.Controllers
{
    public class DashboardController : Controller
    {

        //1-Properties:
        private readonly ReversScaffoldedStoreContext _context;
        private readonly ReversScaffoldedStoreIdentityContext _CustomersIdentitycontext;

        //2-Constructor:
        public DashboardController(ReversScaffoldedStoreContext context, ReversScaffoldedStoreIdentityContext CustomersIdentitycontext)
        {
            _context = context;
            _CustomersIdentitycontext = CustomersIdentitycontext;
        }


        public IActionResult Index()
        {


            //Totals:
            int _TotalProducts = _context.Products.Count();
            int _TotalOrders = _context.Orders.Count();
            int _TotalCustomers = _CustomersIdentitycontext.AspNetUsers.Count();
            int _TotalSoldItems = _context.OrderItems.Select(x => x.Quantity).Sum();
            int _TotalBrands = _context.ProductBrands.Count();
            int _TotalTypes = _context.ProductTypes.Count();










            //use linq queries to do the following:
            var _ProductsPerBrand = (from p in _context.Products
                                     join c in _context.ProductBrands
                                     on p.ProductBrandId equals c.Id into j1
                                     from j2 in j1.DefaultIfEmpty()
                                     select new
                                     {
                                         BrandName = p.ProductBrand.Name,
                                         IncrementProductsCount = j2 == null ? 0 : 1
                                     })
                                     .GroupBy(o => o.BrandName)
                                     .Select(o => new { BrandName = o.Key, Count = o.Sum(p => p.IncrementProductsCount) }).ToList();
            //then convert it to a list of ProductsPerBrand object:
            var _ProductsPerBrandList = new List<ProductsPerBrandObject>();
            foreach(var item in _ProductsPerBrand)
            {
                var ProductsPerBrandObject = new ProductsPerBrandObject
                {
                    BrandName = item.BrandName,
                    Count = item.Count
                };
                _ProductsPerBrandList.Add(ProductsPerBrandObject);
            }










            //use linq queries to do the following:
            var _ProductsPerType = (from p in _context.Products
                                    join c in _context.ProductTypes
                                    on p.ProductTypeId equals c.Id into j1
                                    from j2 in j1.DefaultIfEmpty()
                                    select new
                                    {
                                        TypeName = p.ProductType.Name,
                                        IncrementProductsCount = j2 == null ? 0 : 1
                                    })
                                     .GroupBy(o => o.TypeName)
                                     .Select(o => new { BrandName = o.Key, Count = o.Sum(p => p.IncrementProductsCount) }).ToList();
            //then convert it to a list of ProductsPerTypeObject:
            var _ProductsPerTypeList = new List<ProductsPerTypeObject>();
            foreach (var item in _ProductsPerType)
            {
                var ProductsPerTypeObject = new ProductsPerTypeObject
                {
                    TypeName = item.BrandName,
                    Count = item.Count
                };
                _ProductsPerTypeList.Add(ProductsPerTypeObject);
            }






            //use linq queries to do the following:
            var _Top5SoldProducts = (from oi in _context.OrderItems
                                    select new
                                    {
                                        ProductName= oi.ItemOrderedProductName,
                                        IncrementProductsCount = 1
                                    })
                                    .GroupBy(o => o.ProductName)
                                    .Select(o => new { ProductName = o.Key, Count = o.Sum(p => p.IncrementProductsCount) })
                                    .OrderByDescending(o => o.Count)
                                    .Take(5).ToList();
            //then convert it to a list of Top5SoldProductsObject:
            var _Top5SoldProductsList = new List<Top5SoldProductsObject>();
            foreach(var item in _Top5SoldProducts)
            {
                var Top5SoldProductsObject = new Top5SoldProductsObject
                {
                    ProductName = item.ProductName,
                    Count = item.Count
                };
                _Top5SoldProductsList.Add(Top5SoldProductsObject);
            }
















            //use linq queries to do the following:
            var _Top5BuyingCustomers = (from o in _context.Orders
                                        select new
                                        {
                                            CustomerName = o.BuyerEmail,
                                            AmountPaid = o.Subtotal
                                        })
                                    .GroupBy(o => o.CustomerName)
                                    .Select(o => new { CustomerName = o.Key, AmountPaidSum = o.Sum(p => p.AmountPaid) })
                                    .OrderByDescending(o => o.AmountPaidSum)
                                    .Take(5).ToList();
            //then convert it to a list of Top5BuyingCustomersObject:
            var _Top5BuyingCustomersList = new List<Top5BuyingCustomersObject>();
            foreach(var item in _Top5BuyingCustomers)
            {
                var Top5BuyingCustomersObject = new Top5BuyingCustomersObject
                {
                    CustomerName = item.CustomerName,
                    AmountPaidSum = item.AmountPaidSum
                };
                _Top5BuyingCustomersList.Add(Top5BuyingCustomersObject);
            }
























            //use linq queries to do the following:
            var _Last30DaysWhenOrdersTookPlace = (from o in _context.Orders
                                                 select new
                                                 {
                                                     Date = o.OrderDate.Date,
                                                     Amount = o.Subtotal
                                                 })
                                                 .GroupBy(o => o.Date)
                                                 .Select(o => new { Date = o.Key, AmountSum = o.Sum(p => p.Amount) })
                                                 .OrderBy(o => o.Date)
                                                 .Take(30).ToList();
            //then convert it to a list of Last30DaysWhenOrdersTookPlaceObject:
            var _Last30DaysWhenOrdersTookPlaceList = new List<Last30DaysWhenOrdersTookPlaceObject>();
            decimal _TotalOf_Last30DaysWhenOrdersTookPlaceList = 0;
            foreach (var item in _Last30DaysWhenOrdersTookPlace)
            {
                var Last30DaysWhenOrdersTookPlaceObject = new Last30DaysWhenOrdersTookPlaceObject
                {
                    Date = item.Date,
                    AmountSum = item.AmountSum
                };
                _Last30DaysWhenOrdersTookPlaceList.Add(Last30DaysWhenOrdersTookPlaceObject);

                _TotalOf_Last30DaysWhenOrdersTookPlaceList += item.AmountSum;
            }





            





            //Now take an instance of the DshboardViewModel and fill it with the data above and send it to the View:
            var DashboardViewModel = new DashboardViewModel
            {
                TotalProducts = _TotalProducts,
                TotalOrders = _TotalOrders,
                TotalCustomers = _TotalCustomers,
                TotalSoldItems = _TotalSoldItems,
                TotalBrands = _TotalBrands,
                TotalTypes = _TotalTypes,
                ProductsPerBrand = _ProductsPerBrandList,
                ProductsPerType = _ProductsPerTypeList,
                Top5SoldProducts = _Top5SoldProductsList,
                Top5BuyingCustomers = _Top5BuyingCustomersList,
                Last30DaysWhenOrdersTookPlace = _Last30DaysWhenOrdersTookPlaceList,
                TotalOf_Last30DaysWhenOrdersTookPlaceList = _TotalOf_Last30DaysWhenOrdersTookPlaceList
            };


            return View(DashboardViewModel);
        }

    
    }
}
