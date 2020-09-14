using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

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
                string data = "";
                if (rsa.PrivateKeyXML == null)
                {
                    return Json(new { isError = "Chưa có khóa bí mật" }, JsonRequestBehavior.AllowGet);
                }
                string[] separator = { ",$$$$$" };
                var splitkey = key.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                //string dulieuLayDuoc = Common.FromHexString(key);
                int id = int.Parse(splitkey[0]);
                var check = db.Users.FirstOrDefault(n => n.Id == id);
                if ( check != null)
                {
                    string HoTen = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[1])); 
                    DateTime? NgaySinh = DateTime.Parse(splitkey[2]);
                    string NoiCuTru = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[3]));
                    string QuocGia = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[4]));
                    if (check.HoTen != HoTen && check.NgaySinh != NgaySinh && check.NoiCuTru != NoiCuTru && check.QuocGia != QuocGia)
                    {
                        return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
                    }
                    data =check.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(check.HoTen)) + ",$$$$$ " + (check.NgaySinh != null ? check.NgaySinh.ToString() : "") + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(check.NoiCuTru)) + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(check.QuocGia));
                }
                else
                {
                    return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
                }
                var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, splitkey[5]);
                if (!string.IsNullOrEmpty(bangroD) && !string.IsNullOrEmpty(data))
                {
                    if (bangroD.Equals(Encoding.Unicode.GetString(Common.HashString(data))))
                    {
                        return Json(new { isSuccess = "Qrcode chính xác", DuLieu = check }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }
           catch (Exception ex)
            {
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }     
        }
    }
}