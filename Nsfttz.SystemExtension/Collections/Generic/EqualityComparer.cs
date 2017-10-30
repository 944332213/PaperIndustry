using System.Linq;
using System.Reflection;

namespace System.Collections.Generic
{
    /// <summary>
    /// 字符串比较器
    /// 采用StringComparison进行区域，大小及排序方式进行比较
    /// </summary>
    public class StringEqualityComparer : IEqualityComparer<string>
    {
        public StringEqualityComparer()
        {
            StringComparison = StringComparison.CurrentCulture;
        }

        public StringEqualityComparer(StringComparison comparison)
        {
            StringComparison = comparison;
        }

        public StringComparison StringComparison { get; set; }

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison);
        }

        public int GetHashCode(string obj)
        {
            return EqualityComparer<string>.Default.GetHashCode(obj);
        }
    }

    /// <summary>
    /// 属性比较器
    /// 将对象减缩为比较某一标识进行比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AttributeEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        private readonly Func<T, TKey> _keySelector;

        public AttributeEqualityComparer(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return EqualityComparer<TKey>.Default.Equals(_keySelector(x), _keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<TKey>.Default.GetHashCode(_keySelector(obj));
        }
    }

    /// <summary>
    /// 转换比较器
    /// 将对象转换为枚举集进行遍历枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ConvertEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        private readonly Func<T, IEnumerable<TKey>> _keySelector;

        public ConvertEqualityComparer(Func<T, IEnumerable<TKey>> keySelector)
        {
            _keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            var ax = _keySelector(x);
            var ay = _keySelector(y);
            if (ax == null || ay == null)
            {
                return ax == null && ay == null;
            }
            var xList = ax as TKey[] ?? ax.ToArray();
            var yList = ay as IList<TKey> ?? ay.ToList();
            if (xList.Length != yList.Count)
            {
                return false;
            }
            foreach (var xKey in xList)
            {
                var isEquals = false;
                for (int j = 0; j < yList.Count; j++)
                {
                    var yKey = yList[j];
                    if (EqualityComparer<TKey>.Default.Equals(xKey, yKey))
                    {
                        isEquals = true;
                        yList.RemoveAt(j);
                        break;
                    }
                }
                if (!isEquals)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(T obj)
        {
            var a = _keySelector(obj);
            if (a == null)
            {
                return 0;
            }
            return a.Select(item => EqualityComparer<TKey>.Default.GetHashCode(item)).Sum();
        }
    }

    /// <summary>
    /// 性质比较器
    /// 将列举对象所有公开实例属性进行比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        private readonly PropertyInfo[] _propertyInfo;

        public PropertyEqualityComparer(IEnumerable<PropertyInfo> propertyInfo)
        {
            _propertyInfo = propertyInfo.ToArray();
        }

        public bool Equals(T x, T y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }
            if (_propertyInfo == null || _propertyInfo.Length <= 0)
            {
                return EqualityComparer<T>.Default.Equals(x, y);
            }
            foreach (var propertyInfo in _propertyInfo)
            {
                var valueX = propertyInfo.GetValue(x);
                var valueY = propertyInfo.GetValue(x);
                if (valueX == null)
                {
                    if (valueY != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!valueX.Equals(valueY))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(obj);
        }
    }
}