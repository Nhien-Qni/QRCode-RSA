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
            db = new QRCodeEntities();
            rsa = new Tool.TaoMa();
        }
        // GET: RSA
        public ActionResult Index()
        {
            if (db.RSAs.FirstOrDefault() != null)
                return View(db.RSAs.FirstOrDefault());
            return View();
        }
        public ActionResult TaoKey(string data)
        {
            try
            {
                rsa.AssignNewKey(data);
                if (db.RSAs.FirstOrDefault() == null)
                {
                    db.RSAs.Add(new RSA() { Key = data, PrivateKey = rsa.PrivateKeyXML, PublicKey = rsa.PublicOnlyKeyXML });
                }
                else
                {
                    var duLieu = db.RSAs.FirstOrDefault();
                    duLieu.Key = data;
                    duLieu.PrivateKey = rsa.PrivateKeyXML;
                    duLieu.PublicKey = rsa.PublicOnlyKeyXML;
                   
                }
                db.SaveChanges();
                return Json(rsa, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = "Có lỗi khi tạo khóa."}, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}