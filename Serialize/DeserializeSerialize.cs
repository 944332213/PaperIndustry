using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nsfttz.Common.Serialize.JsonConvert;
using Nsfttz.Common.Serialize.SerializeConverter;

namespace Nsfttz.Common.Serialize
{
    /// <summary>
    /// 并行化序列化
    /// </summary>
    public static class DeserializeSerialize
    {
        private static JavaScriptSerializer _javaScriptSerializer;
        private static BinaryFormatter _binaryFormatter;

        static DeserializeSerialize()
        {
            Newtonsoft.Json.JsonConvert.DefaultSettings = JsonConvertKeymaker.Get;
        }

        #region 成员属性

        /// <summary>
        /// 解析对象
        /// </summary>
        public static JavaScriptSerializer JavaScriptSerializer
        {
            get
            {
                if (_javaScriptSerializer == null)
                {
                    _javaScriptSerializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                    _javaScriptSerializer.RegisterConverters(JavaScriptConverterKeymaker.Get());
                }
                return _javaScriptSerializer;
            }
        }

        /// <summary>
        /// 解析对象
        /// </summary>
        public static BinaryFormatter BinaryFormatter
        {
            get { return _binaryFormatter ?? (_binaryFormatter = new BinaryFormatter()); }
        }

        #endregion

        #region 并行化

        /// <summary>
        /// 并行化值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static T Deserialize<T>(dynamic value, JsonSerializerSettings setting)
        {
            if (Equals(value, null))
            {
                return default(T);
            }
            if (value is T)
            {
                return value;
            }
            return setting == null
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value is String ? value.ToString() : Serialize(value))
                : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(
                    value is String ? value.ToString() : Serialize(value, setting), setting);
        }

        /// <summary>
        /// 并行化值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static T Deserialize<T>(dynamic value, IContractResolver resolver)
        {
            if (resolver == null)
            {
                return Deserialize<T>(value);
            }
            var setting = JsonConvertKeymaker.Get();
            setting.ContractResolver = resolver;
            return Deserialize<T>(value, setting);
        }

        /// <summary>
        /// 并行化值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T Deserialize<T>(dynamic value)
        {
            return Deserialize<T>(value, (JsonSerializerSettings)null);
        }

        #endregion

        #region 序列化

        ///// <summary>
        ///// 序列化
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string Serialize(object value)
        //{
        //    if (value == null)
        //    {
        //        return null;
        //    }
        //    return value is String || value.GetType().IsValueType ? value.ToString() : JavaScriptSerializer.Serialize(value);
        //}

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string Serialize(dynamic value, JsonSerializerSettings setting)
        {
            if (Equals(value, null))
            {
                return null;
            }
            return value is String
                ? value.ToString()
                : setting == null
                    ? Newtonsoft.Json.JsonConvert.SerializeObject(value)
                    : Newtonsoft.Json.JsonConvert.SerializeObject(value, setting);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static string Serialize(dynamic value, IContractResolver resolver)
        {
            if (resolver == null)
            {
                return Serialize(value);
            }
            var setting = JsonConvertKeymaker.Get();
            setting.ContractResolver = resolver;
            return value is String ? value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(value, setting);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(dynamic value)
        {
            return Serialize(value, (JsonSerializerSettings)null);
        }

        /// <summary>
        /// 序列化js回调
        /// </summary>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static string SerializeCallback(dynamic value, string callback)
        {
            return string.IsNullOrWhiteSpace(callback)
                ? Serialize(value)
                : string.Format("{0}({1})", callback, Serialize(value));
        }

        #endregion

    }
}
