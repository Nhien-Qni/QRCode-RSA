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
        public Tool.TaoMa rsa;
        public TaoMaController()
        {
            rsa = new Tool.TaoMa();
        }
        QRCodeEntities db = new QRCodeEntities();
        
        // GET: TaoMa
        public ActionResult Index()
        {
            db.Users.ToList();
          
            return View(db.Users.ToList());
        }
        [HttpPost]
        public ActionResult TaoQR(string data)
        {
            // Tạo PublicKey, PrivateKey
            rsa.AssignNewKey("PassW0rd@123");
            byte[] duLieuBam = Common.HashString(data);
            //var t = Convert.ToBase64String(duLieuBam);
            var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
            string TaoQR = "Họ tên: " + data + ", " + duLieuMaHoa;
            //string TaoQR = Common.FromHexString(duLieuMaHoa);
            return Json(Common.TaoQRCode(TaoQR), JsonRequestBehavior.AllowGet);
        }
       
         
    }
}