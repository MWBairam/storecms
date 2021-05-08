using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.CMSUsersMgmt
{
    public class EditUserViewModel
    {

        //1-Properties:
        public string Email { get; set; }


        [Required(ErrorMessage ="DisplayName Is Required !")]
        public string DisplayName { get; set; }

    }
}
