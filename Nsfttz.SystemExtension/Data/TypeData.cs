using System.Collections.Generic;
using System.Reflection;

namespace System.Data
{
    /// <summary>
    /// 类型数据
    /// </summary>
    public abstract class TypeData
    {
        private static Dictionary<Type, PropertyInfo[]> _sTypePropertyDictionary;

        /// <summary>
        /// 类型属性字典
        /// </summary>
        protected static Dictionary<Type, PropertyInfo[]> TypePropertyDictionary
        {
            get { return _sTypePropertyDictionary ?? (_sTypePropertyDictionary = new Dictionary<Type, PropertyInfo[]>()); }
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertys(Type type)
        {
            if (type == null)
            {
                throw new Exception("获取类型数据时，不可知的类型");
            }
            PropertyInfo[] propertys;
            if (TypePropertyDictionary.TryGetValue(type, out propertys))
            {
                return propertys;
            }
            propertys = type.GetProperties();
            TypePropertyDictionary.Add(type, propertys);
            return propertys;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertys<T>()
        {
            return GetPropertys(typeof(T));
        }
    }
}
