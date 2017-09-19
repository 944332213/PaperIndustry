using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Linq;
using Lushi.PaperProducts.Model.Table;
using Nsfttz.DataAccessLayer.Client.Config;
using Nsfttz.DataAccessLayer.DataTable.SqlServer.Base.Base;

namespace Lushi.PaperProducts.DataAccessLayer.DataBase
{
    public class ArticleDao : BaseIdentityRepository<ModelArticle>
    {
        protected override string ConnectionString
        {
            get { return ConfigManager.GetAppSetting("LushiPaperProductsConnectionString"); }
        }

        public override string TableName
        {
            get { return "Article"; }
        }

        public List<ModelArticle> SelectListByType(params int[] ids)
        {
            DataBaseTypeData.Reset();
            DataBaseTypeData.Model = new ModelArticle
            {
                Status = EnumStatus.Value.Normal,
            };
            Sql.AddText("SELECT");
            Sql.AddText(DataBaseTypeData.GetAllSelectString());
            Sql.AddText("FROM");
            Sql.AddText(TableName);
            Sql.AddText("WITH(NOLOCK)");
            Sql.AddText("WHERE");
            var whereNames = new[] { "Status" };
            Sql.AddText(string.Join(" AND ", whereNames.Select(item => $"[{item}] = @{item}").Concat(new[]
            {
                ids == null || ids.Length <= 0
                    ? null
                    : ids.Length == 1
                        ? $"TypeId = {ids.First()}"
                        : $"TypeId IN({string.Join(",", ids)})",
            }).Where(item => !string.IsNullOrEmpty(item))));
            Sql.AddParameter(DataBaseTypeData.GetSqlParameters(new PermitImplement<string>(true, whereNames)));
            return this.ExecuteTableToList();
        }
    }
}
