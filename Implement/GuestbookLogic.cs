using Lushi.PaperProducts.DataAccessLayer.DataBase;

namespace Lushi.PaperProducts.BusinessLogicLayer.Implement
{
    /// <summary>
    /// 留言板业务逻辑
    /// </summary>
    public class GuestbookLogic
    {
        protected GuestbookDao Dao => new GuestbookDao();
    }
}
