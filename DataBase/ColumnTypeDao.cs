using Lvshi.PaperProducts.Model.Table;
using Nsfttz.Common.Config;
using Nsfttz.DataAccessLayer.DataTable.SqlServer.Base.Base;

namespace Lvshi.PaperProducts.DataAccessLayer.DataBase
{
    public class ColumnTypeDao : BaseIdentityRepository<ModelColumnType>
    {
        protected override string ConnectionString
        {
            get { return ConfigManager.GetAppSetting("LushiPaperProductsConnectionString"); }
        }

        public override string TableName
        {
            get { return "ColumnType"; }
        }
    }
}
