namespace System
{
    /// <summary>
    /// JavaScript扩展
    /// </summary>
    public static class JavaScriptExtension
    {
        private static DateTime? _sMinDate;
        private static string _sDefaultHref;

        /// <summary>
        /// Javascript日期最小值
        /// </summary>
        public static DateTime MinDate
        {
            get { return _sMinDate ?? (DateTime)(_sMinDate = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc)); }
        }

        /// <summary>
        /// 默认链接
        /// </summary>
        public static string DefaultHref
        {
            get { return _sDefaultHref ?? (_sDefaultHref = "javascript:void(0);"); }
        }

        /// <summary>
        /// 转换为Javascript日期
        /// </summary>
        /// <param name="milliseconds">毫秒数</param>
        /// <returns></returns>
        public static DateTime ToJavascriptDate(this long milliseconds)
        {
            return MinDate.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// 转换为Javascript时间戳[毫秒] 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToJavascriptTimestamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToLocal().ToUniversalTime() - MinDate).TotalMilliseconds;
        }
    }
}
