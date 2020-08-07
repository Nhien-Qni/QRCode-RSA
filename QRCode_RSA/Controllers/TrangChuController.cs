using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult QuetMa(string key)
        {
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