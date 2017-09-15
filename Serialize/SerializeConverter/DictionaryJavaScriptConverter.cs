using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Nsfttz.Common.Serialize.SerializeConverter
{
    public class DictionaryJavaScriptConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            Func<string, object> fnGetKey = delegate(string obj)
            {
                var value = Activator.CreateInstance(type.GetGenericArguments()[0]);
                var parameters = new[] { obj, value };
                type.GetGenericArguments()[0].InvokeMember("TryParse",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, parameters);
                return value;
            };
            return dictionary.ToDictionary(item => fnGetKey(item.Key), item => item.Value);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            foreach (var keyValue in ((dynamic)obj))
            {
                result.Add(keyValue.Key.ToString(), keyValue.Value);
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] { typeof(IDictionary<Int64, object>), typeof(IDictionary<Int32, object>), typeof(IDictionary<Int16, object>), }; }
        }
    }
}
