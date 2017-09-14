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
                view.Column = ColumnTypeLogic.GetListParent();
            });

            return View(view);
        }

    }
}