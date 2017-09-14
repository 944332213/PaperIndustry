namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 转换为本地
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToLocal(this DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                value = DateTime.SpecifyKind(value, DateTimeKind.Local);
            }
            return value.ToLocalTime();
        }
    }
}
