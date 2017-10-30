using System.Collections.Generic;

namespace System.Data.Sql
{
    /// <summary>
    /// Sql数据类型
    /// </summary>
    public static class SqlDataType
    {
        private static Dictionary<Type, SqlDbType> _sSqlTypeList;
        
        /// <summary>
        /// sql与类型关系列表
        /// </summary>
        public static Dictionary<Type, SqlDbType> SqlTypeList
        {
            get
            {
                return _sSqlTypeList ?? (_sSqlTypeList = new Dictionary<Type, SqlDbType>
                {
                    {typeof (Byte), SqlDbType.TinyInt},
                    {typeof (SByte), SqlDbType.SmallInt},
                    {typeof (Char), SqlDbType.Char},
                    {typeof (Boolean), SqlDbType.Bit},
                    {typeof (DateTime), SqlDbType.DateTime},
                    {typeof (Int16), SqlDbType.SmallInt},
                    {typeof (Int32), SqlDbType.Int},
                    {typeof (Int64), SqlDbType.BigInt},
                    {typeof (Single), SqlDbType.Real},
                    {typeof (Double), SqlDbType.Float},
                    {typeof (Decimal), SqlDbType.Decimal},
                    {typeof (Guid), SqlDbType.UniqueIdentifier},
                    {typeof (String), SqlDbType.NText},
                    {typeof (Object), SqlDbType.Variant},
                    {typeof (Byte[]), SqlDbType.Binary}
                });
            }
        }

        #region 获取SQL数据类型

        /// <summary>
        /// 获取SQL数据类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public static SqlDbType GetSqlType(Type type)
        {
            if (type.IsEnum)
            {
                return SqlDbType.Int;
            }
            SqlDbType sqlDbType;
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    sqlDbType = GetSqlType(type.GetGenericArguments()[0]);
                }
                else
                {
                    throw new Exception(string.Format("泛型类型 {0} 是不允许的", type.FullName));
                }
            }
            else
            {
                if (!SqlTypeList.TryGetValue(type, out sqlDbType))
                {
                    if (type.IsArray)
                    {
                        throw new Exception(string.Format("数组类型 {0} 是不允许的", type.FullName));
                    }
                    throw new Exception(string.Format("未找到类型 {0} 对应的SQL数据类型", type.FullName));
                }
            }
            return sqlDbType;
        }

        #endregion

        #region 写入SQL数据类型

        /// <summary>
        /// 写入SQL数据类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="sqlType">SQL数据类型</param>
        public static void SetSqlType(Type type, SqlDbType sqlType)
        {
            SqlDbType sqlDbType;
            if (SqlTypeList.TryGetValue(type, out sqlDbType))
            {
                throw new Exception(string.Format("类型 {0} 对应的SQL数据类型已经存在 {1}", type.FullName, sqlDbType));
            }
            SqlTypeList.Add(typeof(int), SqlDbType.Int);
        }

        #endregion

        #region 是否是数值类型

        /// <summary>
        /// 是否是数值类型
        /// </summary>
        /// <param name="sqlType">SQL数据类型</param>
        /// <returns></returns>
        public static bool IsNumericType(SqlDbType sqlType)
        {
            bool flag;
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.Decimal:
                case SqlDbType.Float:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.Real:
                case SqlDbType.SmallMoney:
                case SqlDbType.TinyInt:
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }

        #endregion

    }
}
