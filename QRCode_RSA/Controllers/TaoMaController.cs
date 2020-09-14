using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using QRCode_RSA.Models;
using QRCode_RSA.Content.ultilities;

namespace QRCode_RSA.Controllers
{
    public class TaoMaController : Controller
    {
        QRCodeEntities db;
        public Tool.TaoMa rsa;
        public TaoMaController()
        {
            db = new QRCodeEntities();
            rsa = new Tool.TaoMa();
            if (db.RSAs.FirstOrDefault() != null)
            {
                rsa.PrivateKeyXML = db.RSAs.FirstOrDefault().PrivateKey;
                rsa.PublicOnlyKeyXML = db.RSAs.FirstOrDefault().PublicKey;
            }

        }
        
        // GET: TaoMa
        public ActionResult Index()
        {
            db.Users.ToList();
          
            return View(db.Users.ToList());
        }
        [HttpPost]
        public ActionResult TaoQR(int id)
        {
            if (rsa.PublicOnlyKeyXML == null)
            {
                return Json(new { isError = "Chưa tạo public key" }, JsonRequestBehavior.AllowGet);
            }
            var user = db.Users.FirstOrDefault(n => n.Id == id);
            var data = user.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.HoTen))  + ",$$$$$ " + (user.NgaySinh != null ? ((DateTime)user.NgaySinh).ToString("dd/MM/yyyy") : "") + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.NoiCuTru)) + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.QuocGia));
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