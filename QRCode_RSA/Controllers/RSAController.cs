using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class RSAController : Controller
    {
        public Tool.TaoMa rsa;
        QRCodeEntities db;
        public RSAController()
        {
            rsa = new Tool.TaoMa();
            db = new QRCodeEntities();
        }
        // GET: RSA
        public ActionResult Index()
        {
            if (db.RSAs.Count() == 0)
                return View(db.RSAs.FirstOrDefault());
            return View();
        }
        public ActionResult TaoKey(string data)
        {
            try
            {
                rsa.AssignNewKey(data);
                if (db.RSAs.Count() == 0)
                {
                    db.RSAs.Add(new RSA() { Key = data, PrivateKey = rsa.PrivateKeyXML, PublicKey = rsa.PublicOnlyKeyXML });
                }
                else
                {
                    var duLieu = db.RSAs.FirstOrDefault();
                    duLieu.Key = data;
                    duLieu.PrivateKey = rsa.PrivateKeyXML;
                    duLieu.PublicKey = rsa.PublicOnlyKeyXML;
                    db.SaveChanges();
                }
               
                return Json(rsa, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { isError = "Có lỗi khi tạo khóa."}, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}