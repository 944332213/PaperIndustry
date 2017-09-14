using System.Collections.Generic;
using Nsfttz.DataAccessLayer.Client.SqlServer;

namespace Nsfttz.DataAccessLayer.DataTable.SqlServer.Base.Base
{
    public static class BaseRepositoryExtension
    {
        /// <summary> 
        /// 在 System.Data.DataSet 中添加或刷新行以匹配使用 System.Data.DataSet 和 System.Data.DataTable 名称的数据源中的行。
        /// 转化为实体集
        /// </summary>
        /// <returns></returns>
        public static List<T> ExecuteTableToList<T>(this BaseRepository<T> baseRepository) where T : class, new()
        {
            return baseRepository.Sql.ExecuteTableToList<T>();
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ExecuteTableToEntity<T>(this BaseRepository<T> baseRepository) where T : class, new()
        {
            return baseRepository.Sql.ExecuteTableToEntity<T>();
        }
    }
}
