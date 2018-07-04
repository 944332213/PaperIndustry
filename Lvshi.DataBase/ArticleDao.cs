using System;
using System.Collections.Generic;
using System.Linq;
using Lvshi.PaperProducts.Model.Table;
using Nsfttz.Common.Config;
using Nsfttz.DataAccessLayer.Repository;
using Nsfttz.DataAccessLayer.Repository.Data;
using Nsfttz.DataAccessLayer.Repository.SqlServer;

namespace Lvshi.PaperProducts.DataAccessLayer.DataBase
{
    public class ArticleDao : SqlServerBaseIdentityRepository<ModelArticle, int>
    {
        protected override string ConnectionString => ConfigManager.GetAppSetting("LushiPaperProductsConnectionString");

        public List<ModelArticle> SelectListByType(params int[] ids)
        {
            DatabaseTypeData.DbParameterData = new ModelArticle
            {
                Status = EnumStatus.Value.Normal,
            };
            Sql.AddText("SELECT");
            Sql.AddText(DatabaseTypeData.GetAllSelectString(DatabaseServerType));
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
            Sql.AddParameter(DatabaseTypeData.GetColumnsByPropertyNames(new PermitImplement<string>(true, whereNames)).Select(item => item.DbParameter));
            return this.ExecuteTableToEntities()?.ToList();
        }
    }
}
