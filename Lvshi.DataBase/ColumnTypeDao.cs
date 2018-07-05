using Lvshi.PaperProducts.Model.Table;
using Nsfttz.Common.Config;
using Nsfttz.DataAccessLayer.Repository.SqlServer;

namespace Lvshi.PaperProducts.DataAccessLayer.DataBase
{
    public class ColumnTypeDao : SqlServerBaseIdentityRepository<ModelColumnType,int>
    {
        protected override string ConnectionString => ConfigManager.GetAppSetting("LushiPaperProductsConnectionString");
    }
}
