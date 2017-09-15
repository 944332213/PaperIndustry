using System;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nsfttz.Common.Serialize.JsonConvert
{
    public class DateTimeJsonConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter _sIsoDateTimeConverter;
        private static DateTimeKind? _sDefaultDateTimeKind;

        public static IsoDateTimeConverter IsoDateTimeConverter
        {
            get
            {
                return _sIsoDateTimeConverter ??
                       (_sIsoDateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" });
            }
        }

        public static DateTimeKind DefaultDateTimeKind
        {
            get
            {
                return (DateTimeKind)(_sDefaultDateTimeKind ?? (_sDefaultDateTimeKind = DateTimeKind.Local));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var dateTime = ((DateTime)value);
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DefaultDateTimeKind);
            }
            writer.WriteValue(dateTime.ToUniversalTime());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = reader.Value;
            if (value == null)
            {
                return null;
            }
            if (value is DateTime)
            {
                return ((DateTime)value).ToLocal();
            }
            if (value is long || Regex.IsMatch(value.ToString(), @"^\d+$"))
            {
                var timestamp = DataConvert.TryParse<long>(value.ToString()) ?? 0;
                if (timestamp > 0)
                {
                    return timestamp.ToJavascriptDate().ToLocal();
                }
            }
            try
            {
                return
                    DeserializeSerialize.JavaScriptSerializer.Deserialize<DateTime>(
                        DeserializeSerialize.JavaScriptSerializer.Serialize(value)).ToLocal();
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}