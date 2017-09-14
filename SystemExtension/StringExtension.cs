
namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// 截断至指定长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="truncationAdditional">截断时追加，默认值为空串</param>
        /// <returns></returns>
        public static string Truncation(this string str, int length, string truncationAdditional = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (length < 0)
            {
                length = 0;
            }
            if (str.Length <= length)
            {
                return str;
            }
            if (truncationAdditional == null)
            {
                truncationAdditional = string.Empty;
            }
            if (truncationAdditional.Length > length)
            {
                truncationAdditional = string.Empty;
            }
            return string.Format("{0}{1}", str.Substring(0, length - truncationAdditional.Length), truncationAdditional);
        }
    }
}
