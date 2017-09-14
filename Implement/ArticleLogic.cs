using Lushi.PaperProducts.DataAccessLayer.DataBase;

namespace Lushi.PaperProducts.BusinessLogicLayer.Implement
{
    /// <summary>
    /// 文章业务逻辑
    /// </summary>
    public class ArticleLogic
    {
        protected ArticleDao Dao => new ArticleDao();
    }
}
