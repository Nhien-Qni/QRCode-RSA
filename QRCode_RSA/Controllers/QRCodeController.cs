using QRCode_RSA.Content.ultilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class QRCodeController : Controller
    {
        public Tool.TaoMa rsa;
        public QRCodeController()
        {
            rsa = new Tool.TaoMa();
        }
        // GET: QRCode
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TaoQR(string data)
        {
            // Tạo PublicKey, PrivateKey
            rsa.AssignNewKey("PassW0rd@123");
            byte[] duLieuBam = Common.HashString(data);
            //var t = Convert.ToBase64String(duLieuBam);
            var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
            string TaoQR = data + ", " + duLieuMaHoa;
            //string TaoQR = Common.FromHexString(duLieuMaHoa);
            return Json(Common.TaoQRCode(TaoQR), JsonRequestBehavior.AllowGet);
        }
    }
}