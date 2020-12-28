using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class QRCodeController : Controller
    {
        QRCodeEntities db;
        public Tool.TaoMa rsa;
        public QRCodeController()
        {
            db = new QRCodeEntities();
            rsa = new Tool.TaoMa();
            if (db.RSAs.FirstOrDefault() != null)
            {
                rsa.PrivateKeyXML = db.RSAs.FirstOrDefault().PrivateKey;
                rsa.PublicOnlyKeyXML = db.RSAs.FirstOrDefault().PublicKey;
            }
          
        }
        // GET: QRCode
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult TaoQR(string data)
        {
            if (rsa.PublicOnlyKeyXML == null)
            {
                return Json(new { isError = "Chưa tạo public key" }, JsonRequestBehavior.AllowGet);
            }
            var match = @"[\n]+";
            if (data.Contains("\n"))
            {
                data = Regex.Replace(data,match,"");
            }
            // Tạo PublicKey, PrivateKey
            byte[] duLieuBam = Common.HashString(data);
            //var t = Convert.ToBase64String(duLieuBam);
            var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
            if (duLieuMaHoa.Contains("Mã hóa thất bại"))
            {
                return Json(new { isError = duLieuMaHoa }, JsonRequestBehavior.AllowGet);
            }
            string TaoQR = data + ",$$$$$ " + duLieuMaHoa;
            //string TaoQR = Common.FromHexString(duLieuMaHoa);
            return Json(Common.TaoQRCode(TaoQR), JsonRequestBehavior.AllowGet);
        }
    }
}