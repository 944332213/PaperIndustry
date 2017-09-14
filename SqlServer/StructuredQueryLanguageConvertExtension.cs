using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Nsfttz.DataAccessLayer.Client.SqlServer
{
    public static class StructuredQueryLanguageConvertExtension
    {
        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static TS? ExecuteScalarTryParse<TS>(this StructuredQueryLanguage sql) where TS : struct
        {
            var obj = sql.ExecuteScalar();
            if (obj == null)
            {
                return null;
            }
            return DataConvert.TryParse<TS>(obj.ToString());
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T ExecuteTableToEntity<T>(this StructuredQueryLanguage sql) where T : class, new()
        {
            using (var dataTable = sql.ExecuteTable())
            {
                return dataTable.ToEntity<T>();
            }
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="selectField"></param>
        /// <param name="retain">默认为True</param>
        /// <returns></returns>
        public static T ExecuteTableToEntity<T>(this StructuredQueryLanguage sql, IEnumerable<string> selectField,
            bool? retain = null) where T : class, new()
        {
            using (var dataTable = sql.ExecuteTable())
            {
                return dataTable.ToEntity<T>(null, null, new PermitImplement<string>().SetPermit(retain ?? true, selectField));
            }
        }

        /// <summary>
        /// 在 System.Data.DataSet 中添加或刷新行以匹配使用 System.Data.DataSet 和 System.Data.DataTable 名称的数据源中的行。
        /// 转化为实体集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> ExecuteTableToList<T>(this StructuredQueryLanguage sql) where T : class, new()
        {
            using (var dataTable = sql.ExecuteTable())
            {
                return dataTable.ToEntityList<T>();
            }
        }

        /// <summary>
        /// 在 System.Data.DataSet 中添加或刷新行以匹配使用 System.Data.DataSet 和 System.Data.DataTable 名称的数据源中的行。
        /// 转化为实体集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="selectField"></param>
        /// <param name="retain">默认为True</param>
        /// <returns></returns>
        public static List<T> ExecuteTableToList<T>(this StructuredQueryLanguage sql, IEnumerable<string> selectField,
            bool? retain = null) where T : class, new()
        {
            using (var dataTable = sql.ExecuteTable())
            {
                return dataTable.ToEntityList<T>(null, null, new PermitImplement<string>().SetPermit(retain ?? true, selectField));
            }
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="index"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static List<T> ExecuteDataSetToList<T>(this StructuredQueryLanguage sql, int index, out DataSet dataSet)
            where T : class, new()
        {
            dataSet = sql.ExecuteDataSet();
            if (dataSet == null || dataSet.Tables.Count <= 0)
            {
                return null;
            }
            var dataTable = dataSet.Tables[index];
            return dataTable.ToEntityList<T>();
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="index"></param>
        /// <param name="dataSet"></param>
        /// <param name="selectField"></param>
        /// <param name="retain">默认为True</param>
        /// <returns></returns>
        public static List<T> ExecuteDataSetToList<T>(this StructuredQueryLanguage sql, int index, out DataSet dataSet,
            IEnumerable<string> selectField, bool? retain = null) where T : class, new()
        {
            dataSet = sql.ExecuteDataSet();
            if (dataSet == null || dataSet.Tables.Count <= 0)
            {
                return null;
            }
            var dataTable = dataSet.Tables[index];
            return dataTable.ToEntityList<T>(null, null, new PermitImplement<string>().SetPermit(retain ?? true, selectField));
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<List<T>> ExecuteDataSetToList<T>(this StructuredQueryLanguage sql) where T : class, new()
        {
            using (var dataSet = sql.ExecuteDataSet())
            {
                return dataSet.Tables.Cast<DataTable>().Select(dataTable => dataTable.ToEntityList<T>()).ToList();
            }
        }

        /// <summary>
        /// 执行语句方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="selectField"></param>
        /// <param name="retain">默认为True</param>
        /// <returns></returns>
        public static List<List<T>> ExecuteDataSetToList<T>(this StructuredQueryLanguage sql,
            IEnumerable<string> selectField, bool? retain = null) where T : class, new()
        {
            using (var dataSet = sql.ExecuteDataSet())
            {
                return
                    dataSet.Tables.Cast<DataTable>()
                        .Select(
                            dataTable =>
                                dataTable.ToEntityList<T>(null, null, new PermitImplement<string>().SetPermit(retain ?? true, selectField)))
                        .ToList();
            }
        }

    }
}