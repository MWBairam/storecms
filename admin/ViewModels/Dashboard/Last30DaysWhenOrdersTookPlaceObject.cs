using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.Dashboard
{
    public class Last30DaysWhenOrdersTookPlaceObject
    {
        //those should match the .select query in DashboardController, var _Last30DaysWhenOrdersTookPlace.
        //This will be used as List<> object type in DashboardViewModel.
        public DateTime Date { get; set; }
        public decimal AmountSum { get; set; }
    }
}
