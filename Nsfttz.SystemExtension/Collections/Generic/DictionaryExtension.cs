namespace System.Collections.Generic
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取字典的值
        /// 值为结构
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue? TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : struct
        {
            TValue value;
            return dictionary == null ? null : dictionary.TryGetValue(key, out value) ? value : (TValue?) null;
        }

        /// <summary>
        /// 获取字典的值
        /// 值为类
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue) where TValue : class
        {
            TValue value;
            return dictionary == null ? defaultValue : dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// 获取字典的值
        /// 值为字符串
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns>未找到键返回null</returns>
        public static string TryGetValue<TKey>(this IDictionary<TKey, string> dictionary, TKey key)
        {
            string value;
            return dictionary == null ? null : dictionary.TryGetValue(key, out value) ? value : null;
        }
    }
}
