using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data
{
    public static class DataConvert
    {
        private static readonly IDictionary<Type, MethodInfo> TryParseMethod;

        static DataConvert()
        {
            TryParseMethod = new Dictionary<Type, MethodInfo>();
        }

        #region 值类型

        private static MethodInfo GetTryParse(Type type)
        {
            var keyType = type;
            if (type.IsEnum)
            {
                keyType = typeof (Enum);
            }
            MethodInfo method = null;
            if (TryParseMethod.ContainsKey(keyType))
            {
                method = TryParseMethod[keyType];
            }
            if (method == null)
            {
                var parameterType = new[] {typeof (String), type.MakeByRefType()};
                var methods =
                    keyType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod)
                        .Where(
                            item =>
                                item.Name == "TryParse" && item.GetParameters().Length == parameterType.Length &&
                                (!item.IsGenericMethod || item.IsGenericMethod && item.GetGenericArguments().Length == 1))
                        .ToArray();
                method = methods.Length <= 0
                    ? null
                    : methods.Length == 1
                        ? methods.First()
                        : methods.FirstOrDefault(
                            item =>
                                item.GetParameters()
                                    .Select((p, i) => p.ParameterType == parameterType[i])
                                    .All(macth => macth));
                if (method == null)
                {
                    throw new Exception(string.Format("类型{0}未找到TryParse({1})方法", type,
                        string.Join(",", parameterType.Select(item => item.ToString()))));
                }
                TryParseMethod.Add(keyType, method);
            }
            return method.IsGenericMethod ? method.MakeGenericMethod(type) : method;
        }

        public static dynamic TryParse(string s, Type type, dynamic @default = null)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            type = DataType.GetIgnoreNullableType(type);
            if (type == typeof (Boolean))
            {
                Boolean value;
                return Boolean.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Char))
            {
                Char value;
                return Char.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (SByte))
            {
                SByte value;
                return SByte.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Byte))
            {
                Byte value;
                return Byte.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Int16))
            {
                Int16 value;
                return Int16.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (UInt16))
            {
                UInt16 value;
                return UInt16.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Int32))
            {
                Int32 value;
                return Int32.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (UInt32))
            {
                UInt32 value;
                return UInt32.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Int64))
            {
                Int64 value;
                return Int64.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (UInt64))
            {
                UInt64 value;
                return UInt64.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Single))
            {
                Single value;
                return Single.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Double))
            {
                Double value;
                return Double.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (Decimal))
            {
                Decimal value;
                return Decimal.TryParse(s, out value) ? value : @default;
            }
            if (type == typeof (DateTime))
            {
                DateTime value;
                if (!DateTime.TryParse(s, out value))
                {
                    return @default;
                }
                return value.ToLocal();
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, s);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var parameters = new object[] {s, null};
            if ((bool) GetTryParse(type).Invoke(null, parameters))
            {
                return parameters.Last();
            }
            return @default;
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T? TryParse<T>(string s) where T : struct
        {
            var type = typeof (T);
            if (type.IsEnum)
            {
                T value;
                return Enum.TryParse(s, out value) ? value : (T?) null;
            }
            return (T?) TryParse(s, type);
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string s, out T value) where T : struct
        {
            var result = TryParse<T>(s);
            value = result ?? default(T);
            return result != null;
        }

        #endregion

        #region 引用类型

        /// <summary>
        /// 设置实体属性
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetEntityProperty(object entity, PropertyInfo propertyInfo, object value)
        {
            if (entity == null || propertyInfo == null || !propertyInfo.CanWrite || value == null ||
                value == DBNull.Value)
            {
                return false;
            }
            var valueType = value.GetType();
            var propertyType = DataType.GetIgnoreNullableType(propertyInfo.PropertyType);
            if (value is DateTime)
            {
                value = ((DateTime)value).ToLocal();
            }
            if (valueType != propertyType && propertyType.IsValueType &&
                (!valueType.IsValueType || propertyInfo.PropertyType != propertyType))
            {
                value = TryParse(value.ToString(), propertyType);
                if (value == null)
                {
                    return false;
                }
            }
            propertyInfo.SetValue(entity, value);
            return true;
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="getValue"></param>
        /// <param name="entity"></param>
        /// <param name="propertyInfoEnumerable"></param>
        /// <returns></returns>
        public static dynamic SetEntityDynamic(Func<PropertyInfo, dynamic> getValue, dynamic entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null)
        {
            if (entity == null)
            {
                throw new Exception("设置实体时，不可知的类型");
            }
            if (propertyInfoEnumerable == null)
            {
                //propertyInfoEnumerable =
                //    entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty |
                //                       BindingFlags.SetProperty);
                propertyInfoEnumerable = TypeData.GetPropertys(entity.GetType());
            }
            foreach (var propertyInfo in propertyInfoEnumerable.Where(item => item.CanWrite))
            {
                SetEntityProperty(entity, propertyInfo, getValue(propertyInfo));
            }
            return entity;
        }

        public static object SetEntityDynamic(Func<PropertyInfo, dynamic> getValue, Type type,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null)
        {
            if (type == null)
            {
                throw new Exception("设置实体时，不可知的类型");
            }
            return SetEntityDynamic(getValue, Activator.CreateInstance(type), propertyInfoEnumerable);
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="entity"></param>
        /// <param name="propertyInfoEnumerable"></param>
        /// <returns></returns>
        public static T SetEntity<T>(Func<PropertyInfo, dynamic> getValue, T entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null) where T : class
        {
            if (entity == null)
            {
                throw new Exception(string.Format("entity{0} 为空", typeof (T)));
            }
            return SetEntityDynamic(getValue, entity, propertyInfoEnumerable);
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getValue"></param>
        /// <param name="propertyInfoEnumerable"></param>
        /// <returns></returns>
        public static T SetEntity<T>(Func<PropertyInfo, dynamic> getValue,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null) where T : class, new()
        {
            return SetEntity(getValue, new T(), propertyInfoEnumerable);
        }

        public static T SetEntity<T>(IDictionary propertyValue, T entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null,
            StringComparison? stringComparison = null) where T : class
        {
            if (propertyValue == null)
            {
                return null;
            }
            return SetEntity(propertyInfo =>
            {
                try
                {
                    return
                        propertyValue[
                            propertyValue.Keys.Cast<dynamic>()
                                .First(
                                    item =>
                                        propertyInfo.Name.Equals((item ?? string.Empty).ToString(),
                                            stringComparison ?? StringComparison.CurrentCultureIgnoreCase))];
                }
                catch (Exception)
                {
                    return null;
                }
            }, entity, propertyInfoEnumerable);
        }

        public static T SetEntity<T>(IDictionary propertyValue, IEnumerable<PropertyInfo> propertyInfoEnumerable = null)
            where T : class, new()
        {
            return SetEntity(propertyValue, new T(), propertyInfoEnumerable);
        }

        public static T SetEntity<T>(IDictionary<string, dynamic> propertyValue, T entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null,
            StringComparison? stringComparison = null) where T : class
        {
            if (propertyValue == null)
            {
                return null;
            }
            return SetEntity(propertyInfo =>
            {
                dynamic value;
                if (!propertyValue.TryGetValue(propertyInfo.Name, out value))
                {
                    var key =
                        propertyValue.Keys.FirstOrDefault(
                            item =>
                                propertyInfo.Name.Equals(item,
                                    stringComparison ?? StringComparison.CurrentCultureIgnoreCase));
                    if (key != null)
                    {
                        value = propertyValue[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                return value;
            }, entity, propertyInfoEnumerable);
        }

        public static T SetEntity<T>(IDictionary<string, dynamic> propertyValue,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null) where T : class, new()
        {
            return SetEntity(propertyValue, new T(), propertyInfoEnumerable);
        }

        public static T SetEntity<T, TKey>(IDictionary<TKey, dynamic> propertyValue, T entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null,
            StringComparison? stringComparison = null)
            where T : class
            where TKey : struct
        {
            if (propertyValue == null)
            {
                return null;
            }
            return SetEntity(propertyInfo =>
            {
                try
                {
                    return
                        propertyValue[
                            propertyValue.Keys.Cast<dynamic>()
                                .First(
                                    item =>
                                        propertyInfo.Name.Equals((item ?? string.Empty).ToString(),
                                            stringComparison ?? StringComparison.CurrentCultureIgnoreCase))];
                }
                catch (Exception)
                {
                    return null;
                }
            }, entity, propertyInfoEnumerable);
        }

        public static T SetEntity<T, TKey>(IDictionary<TKey, dynamic> propertyValue,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null)
            where T : class, new()
            where TKey : struct
        {
            return SetEntity(propertyValue, new T(), propertyInfoEnumerable);
        }

        public static T SetEntity<T>(dynamic propertyValue, T entity,
            IEnumerable<PropertyInfo> propertyInfoEnumerable = null) where T : class
        {
            return SetEntity(propertyInfo =>
            {
                try
                {
                    return propertyValue[propertyInfo.Name];
                }
                catch (Exception)
                {
                    return null;
                }
            }, entity, propertyInfoEnumerable);
        }

        public static T SetEntity<T>(dynamic propertyValue, IEnumerable<PropertyInfo> propertyInfoEnumerable = null)
            where T : class, new()
        {
            return SetEntity(propertyValue, new T(), propertyInfoEnumerable);
        }

        #endregion
    }
}