using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nsfttz.DataAccessLayer.Client.SqlServer
{
    public static class SqlFormat
    {
        #region 数据

        #region 添加

        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="into"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string InsertFormat(string into, string field, string values)
        {
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", into, field, values);
        }

        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="into"></param>
        /// <param name="insertDictionary"></param>
        /// <param name="ignoreQuotesKeys"></param>
        /// <returns></returns>
        public static string InsertFormat(string into, IDictionary<string, dynamic> insertDictionary, params string[] ignoreQuotesKeys)
        {
            var data = VerifyData(insertDictionary, ignoreQuotesKeys);
            return InsertFormat(into, string.Join(" ,", data.Keys), string.Join(" ,", data.Values));
        }

        /// <summary>
        /// 添加模板
        /// 查询返回标识列
        /// </summary>
        /// <param name="into"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string InsertIdentityFormat(string into, string field, string values)
        {
            return string.Format("INSERT INTO {0} ({1}) VALUES ({2}) SELECT SCOPE_IDENTITY()", into, field, values);
        }

        /// <summary>
        /// 添加模板
        /// 查询返回标识列
        /// </summary>
        /// <param name="into"></param>
        /// <param name="insertDictionary"></param>
        /// <param name="ignoreQuotesKeys"></param>
        /// <returns></returns>
        public static string InsertIdentityFormat(string into, IDictionary<string, dynamic> insertDictionary, params string[] ignoreQuotesKeys)
        {
            var data = VerifyData(insertDictionary, ignoreQuotesKeys);
            return InsertIdentityFormat(into, string.Join(" ,", data.Keys), string.Join(" ,", data.Values));
        }

        #endregion

        #region 修改

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="update"></param>
        /// <param name="set"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string UpdateFormat(string update, string set, string where)
        {
            //if (string.IsNullOrEmpty(update) || string.IsNullOrEmpty(set))
            //{
            //    return string.Empty;
            //}
            return string.Format("UPDATE {0} SET {1}{2}", update, set,
                string.IsNullOrEmpty(where) ? string.Empty : string.Format(" WHERE {0}", where));
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="update"></param>
        /// <param name="setDictionary"></param>
        /// <param name="where"></param>
        /// <param name="ignoreQuotesKeys"></param>
        /// <returns></returns>
        public static string UpdateFormat(string update, IDictionary<string, dynamic> setDictionary, string where, params string[] ignoreQuotesKeys)
        {
            return UpdateFormat(update,
                string.Join(", ",
                    VerifyData(setDictionary, ignoreQuotesKeys)
                        .Select(item => string.Format("{0} = {1}", item.Key, item.Value))), where);
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="delete"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string DeleteFormat(string delete, string where)
        {
            //if (string.IsNullOrEmpty(delete))
            //{
            //    return string.Empty;
            //}
            return string.Format("DELETE {0}{1}", delete,
                string.IsNullOrEmpty(where) ? string.Empty : string.Format(" WHERE {0}", where));
        }

        /// <summary>
        /// 截断模板
        /// </summary>
        /// <param name="truncate"></param>
        /// <returns></returns>
        public static string TruncateFormat(string truncate)
        {
            //if (string.IsNullOrEmpty(delete))
            //{
            //    return string.Empty;
            //}
            return string.Format("TRUNCATE TABLE {0}", truncate);
        }

        #endregion

        #region 查询

        #region 列表查询

        /// <summary>
        /// 查询模板
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string SelectFormat(string select, string from, string where, int number = 0)
        {
            var sqlText = new StringBuilder(string.Empty);
            sqlText.Append(string.Format("SELECT{0} {1}", number > 0 ? string.Format(" TOP {0}", number) : string.Empty,
                string.IsNullOrEmpty(select) ? "*" : select));
            if (!string.IsNullOrEmpty(from))
            {
                sqlText.AppendFormat(" FROM {0}", from);
            }
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.AppendFormat(" WHERE {0}", where);
            }
            return sqlText.ToString();
        }

        /// <summary>
        /// 查询模板
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string SelectFormat(string select, string from, string where, string orderBy, int number = 0)
        {
            var sqlText = new StringBuilder(string.Empty);
            sqlText.Append(string.Format("SELECT{0} {1}", number > 0 ? string.Format(" TOP {0}", number) : string.Empty,
                string.IsNullOrEmpty(select) ? "*" : select));
            if (!string.IsNullOrEmpty(from))
            {
                sqlText.AppendFormat(" FROM {0}", from);
            }
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.AppendFormat(" WHERE {0}", where);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.AppendFormat(" ORDER BY {0}", orderBy);
            }
            return sqlText.ToString();
        }

        /// <summary>
        /// 查询模板
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string SelectFormat(string select, string from, string where, string groupBy, string orderBy,
            int number = 0)
        {
            var sqlText = new StringBuilder(string.Empty);
            sqlText.Append(string.Format("SELECT{0} {1}", number > 0 ? string.Format(" TOP {0}", number) : string.Empty,
                string.IsNullOrEmpty(select) ? "*" : select));
            if (!string.IsNullOrEmpty(from))
            {
                sqlText.AppendFormat(" FROM {0}", from);
            }
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.AppendFormat(" WHERE {0}", where);
            }
            if (!string.IsNullOrEmpty(groupBy))
            {
                sqlText.AppendFormat(" GROUP BY {0}", groupBy);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.AppendFormat(" ORDER BY {0}", orderBy);
            }
            return sqlText.ToString();
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页查询模板
        /// 无sql参数
        /// 未验证页数
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNow"></param>
        /// <param name="isCountData">是否统计数据，为false时将忽略isSelectCountData参数</param>
        /// <param name="isSelectCountData">是否查询统计数据</param>
        /// <returns></returns>
        public static string SelectPageFormat(string select, string from, string where, string groupBy,
            string orderBy, int pageSize, int pageNow, bool isCountData, bool isSelectCountData)
        {
            var sqlText = new StringBuilder(string.Empty);
            if (isCountData)
            {
                var declareArray = new[] { "RecordCount", "PageCount" };
                sqlText.AppendFormat(" DECLARE @{0} {1} DECLARE @{2} {3}", declareArray[0], SqlDbType.Int.ToString().ToUpper(),
                    declareArray[1], SqlDbType.Int.ToString().ToUpper());
                sqlText.AppendFormat(" SELECT @{0} = COUNT(0) FROM {1}", declareArray[0], from);
                if (!string.IsNullOrEmpty(where))
                {
                    sqlText.AppendFormat(" WHERE {0}", where);
                }
                sqlText.AppendFormat(" IF(@{0} <= {1}) BEGIN SET @{2} = {1} RETURN END", declareArray[0], 0, declareArray[1]);
                sqlText.AppendFormat(" SET @{0} = CEILING(CAST(@{1} AS {2}) / {3})", declareArray[1], declareArray[0],
                    SqlDbType.Real.ToString().ToUpper(), pageSize);
                if (isSelectCountData)
                {
                    sqlText.AppendFormat(" SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {3}", declareArray[0],
                        declareArray[1], pageNow.ToString(CultureInfo.InvariantCulture), "PageNow");
                }
            }
            if (pageNow == 1)
            {
                sqlText.Append(SelectFormat(select, from, where, groupBy, orderBy, pageSize));
            }
            else
            {
                sqlText.AppendFormat(
                    " SELECT * FROM (SELECT {0}, (ROW_NUMBER() OVER (ORDER BY {1})) AS {2} FROM {3}{4}{5}) AS {6}",
                    string.IsNullOrEmpty(select) ? "*" : select, orderBy, "ContinuousColumn",
                    from,
                    string.IsNullOrEmpty(where)
                        ? string.Empty
                        : string.Format(" WHERE {0}", where),
                    string.IsNullOrEmpty(groupBy)
                        ? string.Empty
                        : string.Format(" GROUP BY {0}", groupBy), "Page");
                sqlText.AppendFormat(" WHERE {0} > {1} AND {2} <= {3}", "Page.ContinuousColumn",
                    ((pageNow - 1) * pageSize), "Page.ContinuousColumn", (pageNow * pageSize));
            }
            return sqlText.ToString();
        }

        /// <summary>
        /// 分页查询模板
        /// sql参数:@RecordCount,@PageAll,@PageNow
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="isSelectCountData">是否查询统计数据</param>
        /// <param name="isFirstPage">是否是第一页</param>
        /// <returns></returns>
        public static string SelectPageParameterFormat(string select, string from, string where, string groupBy,
            string orderBy, int pageSize, bool isSelectCountData, bool isFirstPage)
        {
            var sqlText = new StringBuilder(string.Empty);
            var declareArray = new[] { "RecordCount", "PageCount", "PageNow" };
            sqlText.AppendFormat(" SELECT @{0} = COUNT(0) FROM {1}", declareArray[0], from);
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.AppendFormat(" WHERE {0}", where);
            }
            sqlText.AppendFormat(" IF(@{0} <= {1}) BEGIN SET @{2} = {1} RETURN END", declareArray[0], 0, declareArray[1]);
            sqlText.AppendFormat(" SET @{0} = CEILING(CAST(@{1} AS {2}) / {3})", declareArray[1], declareArray[0],
                SqlDbType.Real.ToString().ToUpper(), pageSize);

            if (isFirstPage)
            {
                sqlText.AppendFormat(" SET @{0} = {1}", declareArray[2], 1);
                if (isSelectCountData)
                {
                    sqlText.AppendFormat(" SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {2}", declareArray[0],
                        declareArray[1], declareArray[2]);
                }
                sqlText.Append(SelectFormat(select, from, where, groupBy, orderBy, pageSize));
            }
            else
            {
                sqlText.AppendFormat(
                    " IF(@{0} IS NULL OR @{0} < {1}) BEGIN SET @{0} = {1} END ELSE IF(@{0} > @{2}) BEGIN RETURN END",
                    declareArray[2], 1, declareArray[1]);
                if (isSelectCountData)
                {
                    sqlText.AppendFormat(" SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {2}", declareArray[0],
                        declareArray[1], declareArray[2]);
                }
                sqlText.AppendFormat(
                    " SELECT * FROM (SELECT {0}, (ROW_NUMBER() OVER (ORDER BY {1})) AS {2} FROM {3}{4}{5}) AS {6}",
                    string.IsNullOrEmpty(select) ? "*" : select, orderBy, "ContinuousColumn",
                    from,
                    string.IsNullOrEmpty(where)
                        ? string.Empty
                        : string.Format(" WHERE {0}", where),
                    string.IsNullOrEmpty(groupBy)
                        ? string.Empty
                        : string.Format(" GROUP BY {0}", groupBy), "Page");
                sqlText.AppendFormat(" WHERE {0} > {1} * (@{2} - {3}) AND {4} <= {1} * @{2}", "Page.ContinuousColumn",
                    pageSize, declareArray[2], 1, "Page.ContinuousColumn");
            }
            return sqlText.ToString();
        }

        /// <summary>
        /// 分页查询模板
        /// sql参数:@RecordCount,@PageAll,@PageNow
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="isSelectCountData">是否查询统计数据</param>
        /// <returns></returns>
        public static string SelectPageParameterFormat(string select, string from, string where, string groupBy,
            string orderBy, int pageSize, bool isSelectCountData)
        {
            var sqlText = new StringBuilder(string.Empty);
            var declareArray = new[] { "RecordCount", "PageCount", "PageNow" };
            sqlText.AppendFormat(" SELECT @{0} = COUNT(0) FROM {1}", declareArray[0], from);
            if (!string.IsNullOrEmpty(where))
            {
                sqlText.AppendFormat(" WHERE {0}", where);
            }
            sqlText.AppendFormat(" IF(@{0} <= {1}) BEGIN SET @{2} = {1} RETURN END", declareArray[0], 0, declareArray[1]);
            sqlText.AppendFormat(" SET @{0} = CEILING(CAST(@{1} AS {2}) / {3})", declareArray[1], declareArray[0],
                SqlDbType.Real.ToString().ToUpper(), pageSize);

            sqlText.AppendFormat(
                " IF(@{0} IS NULL OR @{0} < {1}) BEGIN SET @{0} = {1} END ELSE IF(@{0} > @{2}) BEGIN RETURN END",
                declareArray[2], 1, declareArray[1]);
            if (isSelectCountData)
            {
                sqlText.AppendFormat(" SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {2}", declareArray[0],
                    declareArray[1], declareArray[2]);
            }
            sqlText.AppendFormat("IF(@{0} == {1}) BEGIN", declareArray[2], 1);

            #region 首页
            sqlText.Append(SelectFormat(select, from, where, groupBy, orderBy, pageSize));
            #endregion

            sqlText.Append(" END ELSE BEGIN");

            #region 分页
            sqlText.AppendFormat(
                " SELECT * FROM (SELECT {0}, (ROW_NUMBER() OVER (ORDER BY {1})) AS {2} FROM {3}{4}{5}) AS {6}",
                string.IsNullOrEmpty(select) ? "*" : select, orderBy, "ContinuousColumn",
                from,
                string.IsNullOrEmpty(where)
                    ? string.Empty
                    : string.Format(" WHERE {0}", where),
                string.IsNullOrEmpty(groupBy)
                    ? string.Empty
                    : string.Format(" GROUP BY {0}", groupBy), "Page");
            sqlText.AppendFormat(" WHERE {0} > {1} * (@{2} - {3}) AND {4} <= {1} * @{2}", "Page.ContinuousColumn",
                pageSize, declareArray[2], 1, "Page.ContinuousColumn");
            #endregion

            sqlText.Append(" END");
            return sqlText.ToString();
        }

        #endregion

        #endregion

        #endregion

        #region 验证条件

        /// <summary>
        /// 验证条件与
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static StringBuilder CheckWhereAnd(StringBuilder @where)
        {
            if (where.Length > 0)
            {
                where.Append(" AND ");
            }
            return where;
        }

        /// <summary>
        /// 验证条件或
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static StringBuilder CheckWhereOr(StringBuilder @where)
        {
            if (where.Length > 0)
            {
                where.Append(" OR ");
            }
            return where;
        }

        #endregion

        /// <summary>
        /// 转换为SQL语句字符串
        /// 识别枚举，bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreQuotes"></param>
        /// <returns></returns>
        public static string ToSqlString(dynamic value, bool ignoreQuotes)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var type = DataType.GetIgnoreNullableType(value.GetType());
            return type.IsEnum
                ? ((int)value).ToString(CultureInfo.InvariantCulture)
                : value is Boolean
                    ? ((bool)value ? 1 : 0).ToString(CultureInfo.InvariantCulture)
                    : ignoreQuotes
                        ? value.ToString()
                        : string.Format("'{0}'", Regex.Replace(value.ToString(), "'", "''"));
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ignoreQuotesKeys">忽略引号的键，即其值不加引号</param>
        /// <returns></returns>
        public static Dictionary<string, string> VerifyData(IDictionary<string, dynamic> data, string[] ignoreQuotesKeys)
        {
            return data.Where(item => !Equals(item.Value, null))
                .ToDictionary(item => item.Key,
                    item =>
                        (string)ToSqlString(item.Value,
                            ignoreQuotesKeys != null && ignoreQuotesKeys.Length > 0 && ignoreQuotesKeys.Contains(item.Key)));
        }
    }
}
