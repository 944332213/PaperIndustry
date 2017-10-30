using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nsfttz.Common.Serialize.JsonConvert
{
    public static class JsonConvertKeymaker
    {
        private readonly static ICollection<JsonConverter> JsonConverter;

        static JsonConvertKeymaker()
        {
            JsonConverter = new List<JsonConverter>();
        }

        public static JsonSerializerSettings Get()
        {
            var setting = new JsonSerializerSettings
            {
                //空值忽略
                NullValueHandling = NullValueHandling.Ignore,
                //默认值忽略
                //DefaultValueHandling = DefaultValueHandling.Ignore,
                // 解决json序列化时的循环引用问题
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) => { },
            };
            foreach (var jsonConverter in JsonConverter)
            {
                setting.Converters.Add(jsonConverter);
            }
            setting.Converters.Add(new DateTimeJsonConverter());
            return setting;
        }

        public static void Add(JsonConverter jsonConverter)
        {
            JsonConverter.Add(jsonConverter);
        }
    }
}
