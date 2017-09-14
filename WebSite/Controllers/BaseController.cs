using System;
using System.Web.Mvc;

namespace Lushi.PaperProducts.Ui.WebSite.Controllers
{
    public class BaseController : Controller
    {
        public void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                { }
            }
        }
	}
}