using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCode_RSA.Common
{
    public class MenuHelper
    {
        public static bool SingleMenuShow(string key)
        {
            var phanQuyenArr = HttpContext.Current.Session["PhanQuyen"] as List<string>;

            if (phanQuyenArr?.FirstOrDefault(o => o == key) != null)
            {
                return true;
            }
            return false;
        }
    }
}