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
            rsa.AssignNewKey("PassW0rd@123");
            return View();
        }
        [HttpPost]
        public JsonResult QuetMa(string key)
        {
            string dulieuLayDuoc = Common.FromHexString(key);
            var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, dulieuLayDuoc);
            var bangroDbase64 = Convert.ToBase64String(bangroD);
            //if (bangroDbase64.Equals(t))
            //    return View(db.Users.ToList());
            //JsonResult jsonresult;
            //try
            //{
            //    //var splitString = duLieu.Split('$');
            //    //var MaDaiBieu = splitString[1].Trim();
            //    using (var db = new ConnectDB())
            //    {
            //        var checkExist = db.DaiBieus.FirstOrDefault(o => o.MaDaiBieu == key);
            //        if (checkExist != null)
            //        {
            //            checkExist.DiemDanh = true;
            //            db.SaveChanges();
            //            jsonresult = Json(new { Data = true, Message = "Điểm danh thành công", daiBieu = checkExist });
            //            jsonresult.MaxJsonLength = int.MaxValue;
            //        }
            //        jsonresult = Json( new { Data = false, Message = "Không có đại biểu này" });
            //        return jsonresult;
            //    }
            //}
            //catch
            //{
            //    jsonresult = Json(new { Data = false, Message = "Không có đại biểu này" });
            //    return jsonresult;
            //}
            return null;
        }
    }
}