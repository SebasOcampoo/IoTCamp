using CTDWeb.Models;
using System.Web;
using System.Web.Mvc;

namespace CTDWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new AiHandleErrorAttribute());
        }
    }
}
