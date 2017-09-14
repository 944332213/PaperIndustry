﻿using Lushi.PaperProducts.Model.Table;
using Nsfttz.DataAccessLayer.Client.Config;
using Nsfttz.DataAccessLayer.DataTable.SqlServer.Base.Base;

namespace Lushi.PaperProducts.DataAccessLayer.DataBase
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
