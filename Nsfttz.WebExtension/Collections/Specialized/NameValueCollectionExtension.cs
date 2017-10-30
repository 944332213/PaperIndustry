using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtension
    {
        public static Dictionary<string, object> ToDictionary(this NameValueCollection nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                return null;
            }
            var regIsArray = new Regex(@"\[\]$");
            var regIsObject = new Regex(@"^[\[\{]");
            var jss = new JavaScriptSerializer();
            return nameValueCollection.AllKeys.Select(key =>
            {
                var isArray = regIsArray.IsMatch(key);
                var isObject = regIsObject.IsMatch(key);
                var value = nameValueCollection[key];
                return new KeyValuePair<string, object>(isArray ? regIsArray.Replace(key, string.Empty) : key,
                    isArray || isObject
                        ? jss.Deserialize<object>(isArray ? string.Format("[{0}]", value) : value)
                        : value);
            }).ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
