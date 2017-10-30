using System.Web.Mvc;
using System.Web.Routing;

namespace Lvshi.PaperProducts.Ui.Manage
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
