using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// 列举扩展
    /// </summary>
    public static class EnumerableExtension
    {
        #region Distinct

        /// <summary>
        /// 排重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.Distinct(new AttributeEqualityComparer<TSource, TKey>(keySelector));
        }

        /// <summary>
        /// 排重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TKey>> keySelector)
        {
            return source.Distinct(new ConvertEqualityComparer<TSource, TKey>(keySelector));
        }

        #endregion

        #region ToDictionary

        /// <summary>
        /// 转换为字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(item => item.Key, item => item.Value);
        }

        #endregion
    }
}