using Lvshi.PaperProducts.DataAccessLayer.DataBase;

namespace Lvshi.PaperProducts.BusinessLogicLayer.Implement
{
    /// <summary>
    /// 留言板业务逻辑
    /// </summary>
    public class GuestbookLogic
    {
        protected GuestbookDao Dao => new GuestbookDao();
    }
}
