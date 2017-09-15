using System.Web.Mvc;
using Lushi.PaperProducts.BusinessLogicLayer.Implement;
using Lushi.PaperProducts.Ui.WebSite.Model.Home;

namespace Lushi.PaperProducts.Ui.WebSite.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var view = new ModelViewIndex();

            Try(() =>
            {
                view.Column = ColumnTypeLogic.GetListIndex();
            });

            return View(view);
        }
	}
}