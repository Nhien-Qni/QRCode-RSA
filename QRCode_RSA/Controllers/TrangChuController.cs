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
        QRCodeEntities db;
        Tool.TaoMa rsa = new Tool.TaoMa();
        public TrangChuController()
        {
            db = new QRCodeEntities();
            rsa = new Tool.TaoMa();
            if (db.RSAs.FirstOrDefault() != null)
            {
                rsa.PrivateKeyXML = db.RSAs.FirstOrDefault().PrivateKey;
                rsa.PublicOnlyKeyXML = db.RSAs.FirstOrDefault().PublicKey;
            }

        }
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
                if (rsa.PrivateKeyXML == null)
                {
                    return Json(new { isError = "Chưa có khóa bí mật" }, JsonRequestBehavior.AllowGet);
                }
                string[] separator = { ",$$$$$" };
                var splitkey = key.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
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