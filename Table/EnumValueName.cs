using System;
using System.Collections.Generic;
using System.Linq;

namespace Lushi.PaperProducts.Model.Table
{
    public static class EnumValueName
    {
        private static IDictionary<Type, int[]> _sNameValues;

        private static IEnumerable<int> GetNameValues(Type type)
        {
            if ((_sNameValues ?? (_sNameValues = new Dictionary<Type, int[]>())).ContainsKey(type))
            {
                return _sNameValues[type];
            }
            try
            {
                var keys = System.Enum.GetValues(type).Cast<int>();
                var nameValues = keys as int[] ?? keys.ToArray();
                _sNameValues.Add(type, nameValues);
                return nameValues;
            }
            catch (Exception)
            {
                return new int[0];
            }
        }

        public static string ToName<TValue, TName>(this TValue @enum)
            where TValue : struct
            where TName : struct
        {
            var value = (int)(dynamic)@enum;
            if (GetNameValues(typeof(TValue)).Contains(value))
            {
                return ((TName)(dynamic)value).ToString();
            }
            return string.Empty;
        }

        public static string ToName<TValue, TName>(this TValue? @enum)
            where TValue : struct
            where TName : struct
        {
            if (@enum == null)
            {
                return string.Empty;
            }
            return ToName<TValue, TName>((TValue)@enum);
        }
    }
}
