using admin.Models.StoreIdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels
{
    public class AdminResetPasswordForCustomers
    {
        public AdminResetPasswordForCustomers()
        {
            user = new AspNetUser();
        }

        public AspNetUser user { get; set; }
        public string newPassword { get; set; }
    }
}
