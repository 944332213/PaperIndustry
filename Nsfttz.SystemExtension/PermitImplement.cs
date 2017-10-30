using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// 许可器
    /// </summary>
    [Serializable]
    public class PermitImplement<T> : ISerializable, ICloneable
    {
        private readonly List<T> _mForbidList;
        private readonly List<T> _mPassList;

        private bool? _mRetain;
        private IEqualityComparer<T> _mComparer;

        public PermitImplement()
        {
            _mForbidList = new List<T>();
            _mPassList = new List<T>();
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集；）</param>
        /// <param name="limitCollection">限制集</param>
        public PermitImplement(bool retain, IEnumerable<T> limitCollection)
            : this()
        {
            SetPermit(retain, limitCollection);
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集；为null时将采用默认值）</param>
        /// <param name="limitCollection">限制集</param>
        public PermitImplement(bool retain, params T[] limitCollection)
            : this()
        {
            SetPermit(retain, limitCollection);
        }

        protected PermitImplement(SerializationInfo info, StreamingContext context)
        {
            _mPassList = (List<T>)info.GetValue("0", typeof(List<T>));
            _mForbidList = (List<T>)info.GetValue("1", typeof(List<T>));
            _mRetain = (bool?)info.GetValue("2", typeof(bool?));
            _mComparer = (IEqualityComparer<T>)info.GetValue("3", typeof(IEqualityComparer<T>));
        }

        /// <summary>
        /// 比较器
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get { return _mComparer ?? EqualityComparer<T>.Default; }
            set { _mComparer = value; }
        }

        /// <summary>
        /// 指示限制器指令
        /// 为True时许可通过集合
        /// 为False时许可非拒绝集合
        /// 默认为False
        /// </summary>
        public bool Retain
        {
            get
            {
                return _mRetain ?? false;
            }
            private set
            {
                _mRetain = value;
            }
        }

        /// <summary>
        /// 设置许可
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集</param>
        /// <param name="limitCollection">限制集</param>
        /// <returns></returns>
        public PermitImplement<T> SetPermit(bool retain, IEnumerable<T> limitCollection)
        {
            if (limitCollection == null)
            {
                return this;
            }
            var limits = limitCollection as T[] ?? limitCollection.ToArray();
            if (limits.Length <= 0)
            {
                return this;
            }
            foreach (var limit in limits)
            {
                if (_mRetain == null)
                {
                    Retain = retain;
                }
                if (retain)
                {
                    _mPassList.Add(limit);
                    _mForbidList.Remove(limit);
                }
                else
                {
                    _mForbidList.Add(limit);
                    _mPassList.Remove(limit);
                }
            }
            return this;
        }

        /// <summary> 
        /// 设置许可
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集</param>
        /// <param name="limitCollection">限制集</param>
        /// <returns></returns>
        public PermitImplement<T> SetPermit(bool retain, params T[] limitCollection)
        {
            return SetPermit(retain, (IEnumerable<T>)limitCollection);
        }

        /// <summary>
        /// 设置比较器
        /// </summary>
        /// <param name="equalityComparer">为空即为默认值</param>
        /// <returns></returns>
        public PermitImplement<T> SetComparer(IEqualityComparer<T> equalityComparer = null)
        {
            _mComparer = equalityComparer;
            return this;
        }

        /// <summary>
        /// 许可验证
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public bool Permit(T @string)
        {
            return Retain ? _mPassList.Contains(@string, Comparer) : !_mForbidList.Contains(@string, Comparer);
        }

        /// <summary>
        /// 许可验证
        /// </summary>
        /// <param name="strings"></param>
        /// <returns>通过许可的集</returns>
        public IEnumerable<T> Permit(IEnumerable<T> strings)
        {
            return strings.Where(Permit);
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public PermitImplement<T> Reset()
        {
            _mForbidList.Clear();
            _mPassList.Clear();
            _mRetain = null;
            _mComparer = null;
            return this;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("0", _mPassList);
            info.AddValue("1", _mForbidList);
            info.AddValue("2", _mComparer);
            info.AddValue("3", _mRetain);
        }

        public object Clone()
        {
            return ShallowClone();
        }

        public PermitImplement<T> ShallowClone()
        {
            var pi = new PermitImplement<T>();
            if (_mPassList.Count > 0)
            {
                pi.SetPermit(true, _mPassList);
            }
            if (_mForbidList.Count > 0)
            {
                pi.SetPermit(false, _mForbidList);
            }
            return pi;
        }

        public PermitImplement<T> DeepClone()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字符串许可器
    /// </summary>
    [Serializable]
    public class StringPermitImplement : PermitImplement<string>
    {
        public StringPermitImplement() { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集；）</param>
        /// <param name="limitCollection">限制集</param>
        public StringPermitImplement(bool retain, IEnumerable<string> limitCollection) : base(retain, limitCollection) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="retain">是否保留（若为True将许可限制集；若为False将排除限制集；为null时将采用默认值）</param>
        /// <param name="limitCollection">限制集</param>
        public StringPermitImplement(bool retain, params string[] limitCollection) : base(retain, limitCollection) { }

        /// <summary>
        /// 设置比较器
        /// </summary>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public StringPermitImplement SetComparer(StringComparison stringComparison)
        {
            SetComparer(new StringEqualityComparer(stringComparison));
            return this;
        }
    }
}
