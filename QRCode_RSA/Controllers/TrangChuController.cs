using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class TrangChuController : Controller
    {
        QRCodeEntities db = new QRCodeEntities();
        Tool.TaoMa rsa = new Tool.TaoMa();
        // GET: TrangChu
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult QuetMa(string key)
        {
            try
            {
                rsa.AssignNewKey("PassW0rd@123");
                var splitkey = key.Split(',');
                //string dulieuLayDuoc = Common.FromHexString(key);
                var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, splitkey[1]);
                if (!string.IsNullOrEmpty(bangroD))
                {
                    if (bangroD.Equals(System.Text.Encoding.Unicode.GetString(Common.HashString(splitkey[0]))))
                    {
                        return Json(new { isSuccess = "Qrcode chính xác" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }
           catch
            {
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }     
        }
    }
}