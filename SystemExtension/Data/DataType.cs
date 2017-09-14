namespace System.Data
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public static class DataType
    {
        #region 默认值

        /// <summary>
        /// 取得默认值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object Default(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsDefault(object obj, Type type)
        {
            return Equals(obj, Default(type));
        }

        /// <summary>
        /// 是否为空或默认值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNullOrDefault(object obj, Type type)
        {
            return obj == null || IsDefault(obj, type);
        }

        /// <summary>
        /// 是否为空、无意义值或默认值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNullEmptyOrDefault(object obj, Type type)
        {
            if (obj == null)
            {
                return true;
            }
            if (typeof(String) == type)
            {
                return string.IsNullOrWhiteSpace(obj.ToString());
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsNullEmptyOrDefault(obj, type.GetGenericArguments()[0]);
            }
            return IsDefault(obj, type);
        }

        #endregion

        #region 获取类型

        /// <summary>
        /// 获取原始类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetType(Type type)
        {
            if (type.IsGenericType)
            {
                return GetType(type.GetGenericArguments()[0]);
            }
            return type;
        }

        /// <summary>
        /// 获取忽略Nullable类类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetIgnoreNullableType(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        #endregion

    }
}
