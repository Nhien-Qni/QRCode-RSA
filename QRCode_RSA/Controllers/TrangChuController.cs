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
            rsa.AssignNewKey("PassW0rd@123");
            //string dulieuLayDuoc = Common.FromHexString(key);
            var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, key);
            if (!string.IsNullOrEmpty(bangroD))
            {
                foreach (var item in db.Users.ToList())
                {
                    if (bangroD.Equals(System.Text.Encoding.Unicode.GetString(Common.HashString(item.HoTen))))
                    {
                        return Json(item, JsonRequestBehavior.AllowGet);
                    }
                }
            }
           
            //var bangroDbase64 = Convert.ToBase64String(bangroD);
            return Json(new { isError = "QRcode không chính xác"},JsonRequestBehavior.AllowGet);
        }
    }
}