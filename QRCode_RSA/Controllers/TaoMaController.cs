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
            byte[] duLieuBam = HashString(data);
            var t = Convert.ToBase64String(duLieuBam);
            var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
            string TaoQR = Common.ToHexString(duLieuMaHoa);
            return Json(TaoQRCode(TaoQR), JsonRequestBehavior.AllowGet);
        }
        public static byte[] HashString(string data)
        {
            //Declarations
            var sha = SHA256.Create();

            var originalBytes = Encoding.Default.GetBytes(CreateSalt(data));

            return  sha.ComputeHash(originalBytes);
        }
        public FileResultViewModel TaoQRCode(string data)
        {
            //Tạo QRCode
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Jpeg);
                    string filePath = Path.Combine(Server.MapPath("/images"), DateTime.UtcNow.ToBinary() + ".jpg");
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                    var fileVm = new FileResultViewModel();
                    fileVm.Content = Convert.ToBase64String(ms.ToArray());
                    fileVm.FileName = DateTime.Now.ToFileTimeUtc() + ".jpg";
                    return fileVm;
                }
            }
        }
        private static string CreateSalt(string id)
        {
            var userBytes = Encoding.ASCII.GetBytes(id);
            long xored = 0x00;

            foreach (var x in userBytes)
            {
                xored = ++xored ^ x;
            }

            var rand = new Random(Convert.ToInt32(xored));
            var salt = rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            return salt.Substring(salt.Length / 5, salt.Length / 2);
        }
        public class FileResultViewModel
        {
            public string Content { get; set; }
            public string FileName { get; set; }
        }

    }
}