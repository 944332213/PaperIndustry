using System.Collections.Generic;
using System.Data.Sql.Table;
using System.Data.Sql.Table.Attribute;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace System.Data.Sql
{
    /// <summary>
    /// 数据库类型数据
    /// </summary>
    /// <typeparam name="T">数据表实体类型</typeparam>
    public class DataBaseTypeData<T> : TypeData where T : class
    {
        private T _mModel;
        private Dictionary<PropertyInfo, SqlParameter> _mPropertySqlParameterCollection;

        /// <summary>
        /// 模型数据
        /// sql参数值将会匹配对应属性的值
        /// </summary>
        public T Model
        {
            get
            {
                return _mModel;
            }
            set
            {
                SetModel(value);
            }
        }

        /// <summary>
        /// 属性与sql参数集合
        /// 非空
        /// </summary>
        public Dictionary<PropertyInfo, SqlParameter> PropertySqlParameterCollection
        {
            get
            {
                return _mPropertySqlParameterCollection ??
                       (_mPropertySqlParameterCollection =
                           (GetPropertys<T>() ?? new PropertyInfo[0]).ToDictionary(item => item, item =>
                           {
                               if (item.GetCustomAttribute(typeof(NotTableFieldAttribute)) as NotTableFieldAttribute != null)
                               {
                                   return null;
                               }
                               SqlDbType? sqlType = null;
                               var tableFieldInfo =
                                   item.GetCustomAttribute(typeof(TableFieldInfoAttribute)) as TableFieldInfoAttribute;
                               if (tableFieldInfo != null)
                               {
                                   sqlType = tableFieldInfo.Type;
                               }
                               if (sqlType == null)
                               {
                                   try
                                   {
                                       sqlType = SqlDataType.GetSqlType(item.PropertyType);
                                   }
                                   catch (Exception)
                                   {
                                       sqlType = null;
                                   }
                               }
                               object value = null;
                               if (Model != null && item.CanRead)
                               {
                                   value = item.GetValue(Model);
                               }
                               if (sqlType == null)
                               {
                                   if (value != null)
                                   {
                                       return new SqlParameter(item.Name, value);
                                   }
                                   return null;
                               }
                               SqlParameter sqlParameter;
                               switch (sqlType)
                               {
                                   case SqlDbType.Text:
                                       sqlParameter = new SqlParameter(item.Name, SqlDbType.VarChar, -1);
                                       break;
                                   case SqlDbType.NText:
                                       sqlParameter = new SqlParameter(item.Name, SqlDbType.NVarChar, -1);
                                       break;
                                   default:
                                       sqlParameter = new SqlParameter(item.Name, (SqlDbType)sqlType);
                                       break;
                               }
                               if (tableFieldInfo != null)
                               {
                                   if (tableFieldInfo.Size != null)
                                   {
                                       sqlParameter.Size = (int)tableFieldInfo.Size;
                                   }
                               }
                               else
                               {
                                   if (sqlParameter.SqlDbType == SqlDbType.VarChar || sqlParameter.SqlDbType == SqlDbType.NVarChar)
                                   {
                                       sqlParameter.Size = -1;
                                   }
                               }
                               if (value != null)
                               {
                                   sqlParameter.Value = value;
                               }
                               return sqlParameter;
                           }));
            }
        }

        /// <summary>
        /// 属性数组
        /// 非空
        /// </summary>
        public PropertyInfo[] Propertys
        {
            get
            {
                return PropertySqlParameterCollection.Keys.ToArray();
            }
        }

        /// <summary>
        /// sql参数数组
        /// 非空
        /// </summary>
        public SqlParameter[] SqlParameters
        {
            get
            {
                return PropertySqlParameterCollection.Values.Where(item => item != null).ToArray();
            }
        }

        /// <summary>
        /// 设置模型数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataBaseTypeData<T> SetModel(T model)
        {
            _mModel = model;
            foreach (var parameter in PropertySqlParameterCollection)
            {
                if (parameter.Value == null || !parameter.Key.CanRead)
                {
                    continue;
                }
                var value = _mModel == null ? null : parameter.Key.GetValue(model);
                parameter.Value.Value = value ?? DBNull.Value;
            }
            return this;
        }

        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="predicate">元素是否满足条件</param>
        /// <returns></returns>
        public PropertyInfo[] GetPropertys(Func<PropertyInfo, bool> predicate = null)
        {
            if (predicate == null)
            {
                return Propertys;
            }
            return Propertys.Where(predicate).ToArray();
        }

        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="stringPermitImplement">字符串限制器</param>
        /// <param name="predicateTableFieldInfo">元素是否满足表字段信息条件</param>
        /// <returns></returns>
        public PropertyInfo[] GetPropertys(PermitImplement<string> stringPermitImplement, Func<TableFieldInfoAttribute, bool> predicateTableFieldInfo = null)
        {
            var predicate = stringPermitImplement == null && predicateTableFieldInfo == null
                ? (Func<PropertyInfo, bool>)null
                : item => (stringPermitImplement == null || stringPermitImplement.Permit(item.Name)) &&
                          (predicateTableFieldInfo == null ||
                           predicateTableFieldInfo(
                               item.GetCustomAttribute(typeof(TableFieldInfoAttribute)) as TableFieldInfoAttribute));
            return GetPropertys(predicate);
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo GetProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            return
                GetPropertys()
                    .FirstOrDefault(
                        item => string.Equals(item.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 获取sql参数数组
        /// </summary>
        /// <param name="stringPermitImplement">字符串限制器</param>
        /// <param name="predicate">元素是否满足条件</param>
        /// <returns></returns>
        public SqlParameter[] GetSqlParameters(PermitImplement<string> stringPermitImplement = null, Func<TableFieldLable, bool> predicate = null)
        {
            if (stringPermitImplement == null && predicate == null)
            {
                return SqlParameters;
            }
            return PropertySqlParameterCollection.Where(item =>
            {
                if (item.Value == null)
                {
                    return false;
                }
                if (stringPermitImplement != null && !stringPermitImplement.Permit(item.Key.Name))
                {
                    return false;
                }
                if (predicate != null)
                {
                    var tableFieldInfo =
                        item.Key.GetCustomAttribute(typeof(TableFieldInfoAttribute)) as TableFieldInfoAttribute;
                    return predicate(tableFieldInfo == null ? TableFieldLable.None : tableFieldInfo.Lable);
                }
                return true;
            }).Select(item => item.Value).ToArray();
        }

        /// <summary>
        /// 获取sql参数数组
        /// </summary>
        /// <param name="stringPermitImplement">字符串限制器</param>
        /// <param name="tableFieldLable">数据表字段标签</param>
        /// <returns></returns>
        public SqlParameter[] GetSqlParameters(PermitImplement<string> stringPermitImplement, TableFieldLable tableFieldLable)
        {
            return GetSqlParameters(stringPermitImplement,
                fieldLable => (fieldLable & tableFieldLable) == tableFieldLable);
        }

        /// <summary>
        /// 获取sql参数数组
        /// </summary>
        /// <param name="tableFieldLable">数据表字段标签</param>
        /// <returns></returns>
        public SqlParameter[] GetSqlParameters(TableFieldLable tableFieldLable)
        {
            return GetSqlParameters(null, tableFieldLable);
        }

        /// <summary>
        /// 获取sql参数
        /// </summary>
        /// <param name="sqlParameterName">sql参数名称</param>
        /// <returns></returns>
        public SqlParameter GetSqlParameter(string sqlParameterName)
        {
            if (string.IsNullOrEmpty(sqlParameterName))
            {
                return null;
            }
            return
                GetSqlParameters()
                    .FirstOrDefault(
                        item =>
                            string.Equals(item.ParameterName, sqlParameterName,
                                StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public DataBaseTypeData<T> Reset()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (Model != null)
            {
                Model = null;
            }
            return this;
        }
    }
}
