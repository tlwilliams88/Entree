using System.Web;
using System.Web.Mvc;

namespace KeithLink.Svc.FoundationSvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}