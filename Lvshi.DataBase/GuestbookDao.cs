using Lvshi.PaperProducts.Model.Table;
using Nsfttz.Common.Config;
using Nsfttz.DataAccessLayer.Repository.SqlServer;

namespace Lvshi.PaperProducts.DataAccessLayer.DataBase
{
    public class GuestbookDao : SqlServerBaseIdentityRepository<ModelGuestbook, int>
    {
        protected override string ConnectionString
        {
            get { return ConfigManager.GetAppSetting("LushiPaperProductsConnectionString"); }
        }
    }
}
