using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Models.CmsIdentity
{
    //This class will be used as the microsoft AspNetUsers table's model instead of the original IdentityUser.
    //But we will inherit the original model to use its fields, and we will add an additional filed to it. 
    public class CmsUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
