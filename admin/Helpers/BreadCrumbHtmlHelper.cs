using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace admin.Helpers
{
    public static class BreadCrumbHtmlHelper
    {
        private static readonly HtmlContentBuilder _emptyBuilder = new HtmlContentBuilder();

        public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper)
        {
            //Exclud all the views of the HomeCntroller and ErrorController from having a breadcrumb:
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home" ||
                helper.ViewContext.RouteData.Values["controller"].ToString() == "Error")
            {
                return _emptyBuilder;
            }
            //for the Views of other Controllers, create a breadcrumb from the controller name and action method name:
            string controllerName = helper.ViewContext.RouteData.Values["controller"].ToString();
            //If the Controller Name is Account, make it appear as CMSUser, elseif AccountProfile, make it appear as MyProfule
            //elseif AccountRoles, make it appear as CMSRoles, else keep controllerName string as it is:
            controllerName = controllerName == "Account" ? "CMSUsers" : controllerName == "AccountProfile" ? "MyProfile" : controllerName == "AccountRoles" ? "CMSRoles" : controllerName;

            string actionName = helper.ViewContext.RouteData.Values["action"].ToString();

            var breadcrumb = new HtmlContentBuilder()
                                .AppendHtml("<ol class='breadcrumb'><li>")
                                .AppendHtml(helper.ActionLink("Home", "Index", "Home"))
                                .AppendHtml("</li><li>")
                                .AppendHtml("&nbsp;&nbsp;>>&nbsp;&nbsp;")
                                .AppendHtml(helper.ActionLink(controllerName, "Index", controllerName))
                                .AppendHtml("</li>");

            //if the called method is Index(), do not write /Index in the breadcrumb, and just leav the controller name.
            if (helper.ViewContext.RouteData.Values["action"].ToString() != "Index")
            {
                breadcrumb.AppendHtml("<li>")
                          .AppendHtml(">>")
                          .AppendHtml(helper.ActionLink(actionName, actionName, controllerName))
                          .AppendHtml("</li>");
            }

            return breadcrumb.AppendHtml("</ol>");
        }
    }
}

/*

** What is a breadcrumb ?
-If we are hitting the View of Index() method of the TransactionController, a breadcrumb should appear like:
Home/Transaction (it is not Home/Transaction/Index, no need to write the action method name here).

-If we are hitting the View of EditRow(id) method of the xxxController for example, a breadcrumb should appear like:
Home/xxx/Editrow (here we write the method name in the breadcrumb).

** How we can form that ? with the help of bootstrap 4 ordered list <ol> and class='breadcrumb'.
-For example, for the previous example:
<ol classe="breadcrumb">
  <li><a asp-controller="Home" asp-action="Index"> Home </a></li>
  <li><a asp-controller="xxx" asp-action="Index"> xxx </a></li>
  <li><a asp-controller="xxx" asp-action="EditRow"> Editrow </a></li>
</ol>
-The first two <li> are formed when we first create the "var breadcrumb = new HtmlContentBuilder()".
-The second <li> is formed using the next "if" block.

*/