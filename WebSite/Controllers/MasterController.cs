using System.Web.Mvc;
using Lvshi.PaperProducts.BusinessLogicLayer.Implement;
using Lvshi.PaperProducts.Ui.WebSite.Model.Master;

namespace Lvshi.PaperProducts.Ui.WebSite.Controllers
{
    public class MasterController : BaseController
    {
        //
        // GET: /Master/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Head()
        {
            var view = new ModelViewHead();

            Try(() =>
            {
                view.Navigation = ColumnTypeLogic.GetListNavigation();
            });

            return View(view);
        }

        public ActionResult Bottom()
        {
            return View();
        }

    }
}