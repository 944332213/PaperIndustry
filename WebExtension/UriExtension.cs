using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    public static class UriExtension
    {
        /// <summary>
        /// 合并Url参数
        /// </summary>
        /// <param name="queryDictionary"></param>
        /// <returns></returns>
        public static string CompoundUrlParameter(IDictionary<string, string> queryDictionary)
        {
            if (queryDictionary == null || queryDictionary.All(item => string.IsNullOrEmpty(item.Value)))
            {
                return string.Empty;
            }
            return string.Join("&",
                queryDictionary.Where(item => !string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                    .Select(keyValue =>
                    {
                        if (string.IsNullOrEmpty(keyValue.Key))
                        {
                            return keyValue.Value;
                        }
                        return string.Format("{0}={1}", keyValue.Key,
                            string.IsNullOrEmpty(keyValue.Value) ? string.Empty : HttpUtility.UrlEncode(keyValue.Value));
                    }));
        }

        /// <summary>
        /// 拆分Url参数
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static IDictionary<string, string> AnalysisUrlParameter(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }
            var regEquest = new Regex("(?<=^[^=]*?)=(?=.*$)", RegexOptions.Singleline);
            return queryString.Split('&')
                .Select(query => regEquest.Split(query))
                .Distinct(item => item[0])
                .ToDictionary(keyValue => keyValue[0],
                    keyValue => keyValue.Length > 1 ? HttpUtility.UrlDecode(keyValue[1]) : string.Empty);
        }

        /// <summary>
        /// 查询字符串字典形式
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Query(this Uri uri)
        {
            return AnalysisUrlParameter(Regex.Replace(uri == null ? string.Empty : uri.Query, @"^\?", string.Empty));
        }

        /// <summary>
        /// 获取协议、主机、端口及绝对路径
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string SchemeAuthorityAbsolutePath(this Uri uri)
        {
            if (uri == null)
            {
                return string.Empty;
            }
            return string.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.AbsolutePath);
        }

        public static Uri SetQuery(this Uri uri, IDictionary<string, string> query, bool isIgnoreOld = false)
        {
            if (uri == null)
            {
                return null;
            }
            if (!isIgnoreOld && query == null || query.Count <= 0)
            {
                return uri;
            }
            var urlParameter =
                CompoundUrlParameter(isIgnoreOld
                    ? query
                    : query.Union(uri.Query()).ToDictionary(item => item.Key, item => item.Value));
            return
                new Uri(string.IsNullOrEmpty(urlParameter)
                    ? uri.SchemeAuthorityAbsolutePath()
                    : string.Format("{0}?{1}", uri.SchemeAuthorityAbsolutePath(), urlParameter));
        }
    }
}
