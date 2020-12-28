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
            if (Session["User"] != null)
            {
                return View(db.Users.ToList());
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public ActionResult TaoQR(int id)
        {
            if (rsa.PublicOnlyKeyXML == null)
            {
                return Json(new { isError = "Chưa tạo public key" }, JsonRequestBehavior.AllowGet);
            }
            var user = db.Users.FirstOrDefault(n => n.Id == id);
            var data = user.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.HoTen)) + ",$$$$$ " + (user.NgaySinh != null ? user.NgaySinh.ToString() : "") + ",$$$$$ " + (user.SoHieu != null ? user.SoHieu.ToString() : "") + ",$$$$$ " + (user.SoBangCap != null ? user.SoBangCap.ToString() : "");
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
        [HttpPost]
        public ActionResult ThemSua(UsersViewModel user)
        {
            int id = user.Id;
            var errors = Validate(user);
            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }
            if (user.AvatarFile != null)
            {
                byte[] fileInBytes = new byte[user.AvatarFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(user.AvatarFile.InputStream))
                {
                    fileInBytes = theReader.ReadBytes(user.AvatarFile.ContentLength);
                }
                user.Avatar = "data:image/jpeg;base64," + Convert.ToBase64String(fileInBytes);
            }
            if (id == 0)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { Id = 0, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var check = db.Users.FirstOrDefault(n => n.Id == user.Id);
                if (check != null)
                {
                    check.HoTen = user.HoTen;
                    check.NgaySinh = user.NgaySinh;
                    check.NoiCuTru = user.NoiCuTru;
                    check.QuocGia = user.QuocGia;
                    if (user.Avatar != null)
                    {
                        check.Avatar = user.Avatar;
                    }
                    db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Id = check.Id, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public ActionResult GetItem(int Id)
        {
            var check = db.Users.FirstOrDefault(n => n.Id == Id);
            return Json(check, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult XoaItem(int Id)
        {
            var check = db.Users.FirstOrDefault(n => n.Id == Id);
            if (check != null)
            {
                db.Users.Remove(check);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }   
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }
        public static List<ValidateError> Validate(User user)
        {
            var errors = new List<ValidateError>();
            if (String.IsNullOrEmpty(user.HoTen) || String.IsNullOrEmpty(user.HoTen.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "HoTen",
                    Description = "Họ tên không được trống."
                });
            }

            if (String.IsNullOrEmpty(user.NoiCuTru) || String.IsNullOrEmpty(user.NoiCuTru.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "NoiCuTru",
                    Description = "Nơi cư trú không được trống."
                });
            }
            if (String.IsNullOrEmpty(user.QuocGia) || String.IsNullOrEmpty(user.QuocGia.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "QuocGia",
                    Description = "Quốc gia không được trống."
                });
            }
            if (!user.NgaySinh.HasValue)
            {
                errors.Add(new ValidateError
                {
                    Id = "NgaySinh",
                    Description = "Ngày sinh không được trống."
                });
            }
            return errors;
        }
    }
}