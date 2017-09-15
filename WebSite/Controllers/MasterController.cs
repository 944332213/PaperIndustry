using System.Web.Mvc;
using Lushi.PaperProducts.BusinessLogicLayer.Implement;
using Lushi.PaperProducts.Ui.WebSite.Model.Master;

namespace Lushi.PaperProducts.Ui.WebSite.Controllers
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