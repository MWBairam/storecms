using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModels.CMSUsersMgmt
{
    public class DisplayRoleInfoViewModel
    {

        //1-Properties:
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }

        //what is the list of users ?
        /*
        What exactly we need to edit in a Role ?!
        we can edit the Role's name, but also we will allow the admin to add users to a role (grant another user this role) 
        or remove users from this role.
        so typically, once we click on Edit button, we go to the Edit Role view. 
        Edit Role view should look like a view that displays  the role name with the associated users.
        */


        //2-Constructor:
        //since we have a List above, we need to initialize it as a new list of strings once we instantiate this class,
        //otherwise, an exception will be thrown:
        public DisplayRoleInfoViewModel()
        {
            Users = new List<string>();
        }
    }
}
