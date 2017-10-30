using System;
using System.Web.Mvc;

namespace Lvshi.PaperProducts.Ui.WebSite.Controllers
{
    public class BaseController : Controller
    {
        public void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Response.Write($"<!--{exception}-->");
            }
        }
    }
}