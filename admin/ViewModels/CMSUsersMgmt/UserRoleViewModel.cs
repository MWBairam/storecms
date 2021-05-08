using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.CMSUsersMgmt
{
    public class UserRoleViewModel
    {
        public string userEmail { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
