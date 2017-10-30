using System;
using System.Collections.Generic;
using System.Linq;

namespace Lvshi.PaperProducts.Model.Table
{
    public static class StructMenuIcon
    {
        private static IDictionary<Key, Value> _sKeyValue;

        private static IDictionary<Key, Value> KeyValue
        {
            get
            {
                int value = 0;
                return _sKeyValue ?? (_sKeyValue = Enum.GetValues(typeof(Key)).Cast<Key>()
                           .ToDictionary(item => item, item => new Value {Key = item, ValueOf = value++}));
            }
        }

        public static Value[] Values
        {
            get
            {
                return KeyValue.Select(item => item.Value).ToArray();
            }
        }

        internal enum Key
        {
            Home,
            Cuturel,
            Brand,
            News,
            Store,
            Join,
            Job,
            Tel,
        }
        public static class Name
        {
            public static IDictionary<Value, string> Dictionary = new Dictionary<Value, string>
            {
                {Value.Home, "首页"},
                {Value.Cuturel, "企业文化"},
                {Value.Brand, "品牌产品"},
                {Value.News, "新闻资讯"},
                {Value.Store, "网上商城"},
                {Value.Join, "招商加盟"},
                {Value.Job, "人才招聘"},
                {Value.Tel, "联系我们"},
            };
        }

        public static string ToName(this Value key)
        {
            return Name.Dictionary.ContainsKey(key) ? Name.Dictionary[key] : key.ToString();
        }

        [Serializable]
        public struct Value
        {
            #region 枚举

            public static Value Home => KeyValue[Key.Home];

            public static Value Cuturel => KeyValue[Key.Cuturel];

            public static Value Brand => KeyValue[Key.Brand];

            public static Value News => KeyValue[Key.News];

            public static Value Store => KeyValue[Key.Store];

            public static Value Join => KeyValue[Key.Join];

            public static Value Job => KeyValue[Key.Job];

            public static Value Tel => KeyValue[Key.Tel];

            #endregion

            private int? _mValueOf;
            private string _mFileName;

            internal Key Key { get; set; }

            public int ValueOf
            {
                get
                {
                    if (_mValueOf == null)
                    {
                        return -1;
                        /*throw new NullReferenceException("当前图标未设置值");*/
                    }
                    return (int)_mValueOf;
                }
                internal set
                {
                    if (_mValueOf == null)
                    {
                        _mValueOf = value;
                    }
                }
            }

            public string FileName
            {
                get
                {
                    if (string.IsNullOrEmpty(_mFileName))
                    {
                        _mFileName = $"{Key}.png".ToLower();
                    }
                    return _mFileName;
                }
                internal set => _mFileName = value;
            }

            public string Url => $"/image/head/menu/{FileName}";

            public bool Equals(Value other)
            {
                return ValueOf == other.ValueOf;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Value && Equals((Value)obj);
            }

            public override int GetHashCode()
            {
                return ValueOf;
            }

            public override string ToString()
            {
                return Key.ToString();
            }

            public static explicit operator Int32(Value value)
            {
                return value.ValueOf;
            }
            public static explicit operator Value(Int32 value)
            {
                return Values.FirstOrDefault(item => item.ValueOf == value);
            }
            public static bool operator ==(Value a, Value b)
            {
                return Equals(a, b);
            }
            public static bool operator !=(Value a, Value b)
            {
                return !Equals(a, b);
            }
        }
    }
}
