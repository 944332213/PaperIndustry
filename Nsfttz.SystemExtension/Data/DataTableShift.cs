using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// DataTable转换
    /// </summary>
    public static class DataTableShift
    {
        private static MethodInfo _toEntityListMethod;

        internal static MethodInfo GetToEntityListMethod(Type type)
        {
            return
                (_toEntityListMethod ??
                 (_toEntityListMethod =
                     typeof(DataTableShift).GetMethod("ToEntityList",
                         BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod))).MakeGenericMethod(type);
        }

        #region 转换类型

        /// <summary>
        /// 转换为实体
        /// </summary>
        /// <param name="dataRow">数据行</param>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="propertyInfoCollection">属性</param>
        /// <param name="stringPermit">许可</param>
        /// <param name="stringComparison">分析器和字段名称与属性的比较方式</param>
        /// <returns></returns>
        public static dynamic ToEntity(this DataRow dataRow, dynamic entity, Type type = null,
            IEnumerable<PropertyInfo> propertyInfoCollection = null, PermitImplement<string> stringPermit = null,
            StringComparison? stringComparison = null)
        {
            if (dataRow == null || dataRow.Table == null)
            {
                return null;
            }
            string[] columnNames = null;
            if (dataRow.Table != null)
            {
                columnNames = dataRow.Table.Columns.Cast<DataColumn>().Select(item => item.ColumnName).ToArray();
            }
            return DataConvert.SetEntityDynamic((Func<PropertyInfo, dynamic>)(pi =>
            {
                var comparison =
                    new StringEqualityComparer(stringComparison ?? StringComparison.CurrentCultureIgnoreCase);
                if (stringPermit != null && !stringPermit.SetComparer(comparison).Permit(pi.Name))
                {
                    return null;
                }
                if (columnNames == null)
                {
                    try
                    {
                        return dataRow[pi.Name];
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                var columnName = columnNames.FirstOrDefault(item => comparison.Equals(item, pi.Name));
                if (columnName == null)
                {
                    return null;
                }
                return dataRow[columnName];
            }), entity ?? type, propertyInfoCollection);
        }

        /// <summary>
        /// 转换为实体
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <param name="dataRow">数据行</param>
        /// <param name="entity"></param>
        /// <param name="propertyInfoCollection">属性</param>
        /// <param name="stringPermit">许可</param>
        /// <param name="stringComparison">分析器和字段名称与属性的比较方式</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dataRow, T entity = null,
            IEnumerable<PropertyInfo> propertyInfoCollection = null, PermitImplement<string> stringPermit = null,
            StringComparison? stringComparison = null) where T : class, new()
        {
            return (T)ToEntity(dataRow, entity ?? new T(), null, propertyInfoCollection, stringPermit, stringComparison);
        }

        /// <summary>
        /// 转换为List
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="getEntity"></param>
        /// <param name="propertyInfoCollection">属性</param>
        /// <param name="stringPermit">许可</param>
        /// <param name="stringComparison">分析器和字段名称与属性的比较方式</param>
        /// <param name="tableUniquenessFieldCollection">数据表唯一标识字段列表</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataTable dataTable, Func<DataRow, T> getEntity = null,
            IEnumerable<PropertyInfo> propertyInfoCollection = null, PermitImplement<string> stringPermit = null,
            StringComparison? stringComparison = null, params ICollection<string>[] tableUniquenessFieldCollection)
            where T : class, new()
        {
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                return null;
            }
            var list = ToEntityList(dataTable, getEntity, propertyInfoCollection, stringPermit, stringComparison,
                tableUniquenessFieldCollection);
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 转换为List
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="getEntity"></param>
        /// <param name="propertyInfoCollection">属性</param>
        /// <param name="stringPermit">许可</param>
        /// <param name="stringComparison">分析器和字段名称与属性的比较方式</param>
        /// <param name="tableUniquenessFieldCollection">数据表唯一标识字段列表</param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable dataTable, Func<DataRow, T> getEntity = null,
            IEnumerable<PropertyInfo> propertyInfoCollection = null, PermitImplement<string> stringPermit = null,
            StringComparison? stringComparison = null, params ICollection<string>[] tableUniquenessFieldCollection)
            where T : class, new()
        {
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                return null;
            }
            if (propertyInfoCollection == null)
            {
                //propertyInfoCollection =
                //    typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty |
                //                       BindingFlags.SetProperty);
                propertyInfoCollection = TypeData.GetPropertys<T>();
            }
            var isOrlop = tableUniquenessFieldCollection == null || tableUniquenessFieldCollection.Length <= 0;
            var list = isOrlop
                ? dataTable.Select()
                    .Select(
                        dr =>
                            ToEntity(dr, getEntity == null ? null : getEntity(dr), propertyInfoCollection, stringPermit,
                                stringComparison))
                    .ToList()
                : dataTable.Select()
                    .Distinct(dr => tableUniquenessFieldCollection[0].Select(key => dr[key]))
                    .Select(
                        dr =>
                            ToEntity(dr, getEntity == null ? null : getEntity(dr), propertyInfoCollection, stringPermit,
                                stringComparison))
                    .ToList();
            if (list.Count <= 0)
            {
                return null;
            }
            if (isOrlop)
            {
                return list;
            }
            var uniquenessWhere = new StringBuilder(string.Empty);
            var propertyInfoGenericCollection =
                propertyInfoCollection.Where(
                    propertyInfo =>
                        (stringPermit == null ||
                         stringPermit.SetComparer(
                             new StringEqualityComparer(stringComparison ?? StringComparison.CurrentCultureIgnoreCase))
                             .Permit(propertyInfo.Name)) && propertyInfo.PropertyType.IsGenericType &&
                        propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof (List<>)).ToArray();
            foreach (var model in list)
            {
                foreach (var propertyInfo in propertyInfoGenericCollection)
                {
                    if (!propertyInfo.CanWrite)
                    {
                        continue;
                    }
                    uniquenessWhere.Clear();
                    foreach (var uniquenessField in tableUniquenessFieldCollection[0])
                    {
                        if (uniquenessWhere.Length > 0)
                        {
                            uniquenessWhere.Append(" AND ");
                        }
                        uniquenessWhere.AppendFormat("{0} = '{1}'", uniquenessField, dataTable.Rows[0][uniquenessField]);
                    }
                    dataTable.TableName = "DataTable";
                    propertyInfo.SetValue(model,
                        GetToEntityListMethod(propertyInfo.PropertyType.GetGenericArguments()[0])
                            .Invoke(null,
                                new object[]
                                {
                                    new DataView {Table = dataTable, RowFilter = uniquenessWhere.ToString()}.ToTable(),
                                    null, null, stringPermit, stringComparison,
                                    tableUniquenessFieldCollection.Skip(1).ToArray()
                                }));
                }
            }
            return list;
        }

        public static Dictionary<string, object> ToDictionary(this DataRow dataRow)
        {
            if (dataRow == null)
            {
                return null;
            }
            if (dataRow.Table == null)
            {
                throw new Exception("该行不属于任何表");
                //return DeserializeSerialize.Deserialize<Dictionary<string, object>>(dataRow);
            }
            return dataRow.Table.Columns.Cast<DataColumn>().ToDictionary(name => name.ColumnName, name => dataRow[name]);
        }

        public static List<Dictionary<string, object>> ToDictionaryList(this DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                return null;
            }
            return dataTable.Select().Select(dataRow => dataRow.ToDictionary()).ToList();
        }

        #endregion
    }
}