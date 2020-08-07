using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Models.HtmlHelpers
{
    public static class UrlVersionHelper
    {
        private static Dictionary<string, string> _cacheVersion;
        public static string ContentVersion(this UrlHelper url, string content)
        {
            // return whatever you want (here's an example)...
            if (_cacheVersion == null)
            {
                _cacheVersion = new Dictionary<string, string>();
            }
            string contentResult;
            if (!_cacheVersion.TryGetValue(content, out contentResult))
            {
                var path = HttpContext.Current.Server.MapPath(content);
                if (File.Exists(path))
                {
                    contentResult = content + "?v=" + File.GetLastWriteTime(path).ToString("yyyyMMddhhmmss");
                    _cacheVersion.Add(content, contentResult);
                }
            }
            return url.Content(string.IsNullOrEmpty(contentResult) ? content : contentResult);
        }
    }
}