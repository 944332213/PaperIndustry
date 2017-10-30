using System.Web.Mvc;
using Lvshi.PaperProducts.BusinessLogicLayer.Implement;
using Lvshi.PaperProducts.Ui.WebSite.Model.Home;

namespace Lvshi.PaperProducts.Ui.WebSite.Controllers
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
                view.Column = new ArticleLogic().GetListTypeArticle(ColumnTypeLogic.GetListIndex());
            });

            return View(view);
        }
    }
}