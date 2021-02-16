using Dashboard.Common;
using Newtonsoft.Json;
using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class TaiKhoanController : Controller
    {
        QRCodeEntities db;
        public TaiKhoanController()
        {
            db = new QRCodeEntities();
        }
        // GET: TaiKhoan
        [IsAuthorize(MenuKey = "TaiKhoan")]
        public ActionResult Index()
        {
            return View(db.TaiKhoans.ToList());
        }
        [HttpPost]
        [IsAuthorize (MenuKey = "TaiKhoan")]
        public ActionResult ThemSua(TaiKhoan taikhoan)
        {
            int id = taikhoan.Id;
            var errors = Validate(taikhoan);
            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }
            if (id == 0)
            {
                try
                {
                    db.TaiKhoans.Add(taikhoan);
                    db.SaveChanges();
                    return Json(new { Id = 0, JsonRequestBehavior.AllowGet });
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                var check = db.TaiKhoans.AsNoTracking().FirstOrDefault(n => n.Id == taikhoan.Id);
                if (check != null)
                {
                    check.Password = taikhoan.Password;
                    check.VaiTroId = taikhoan.VaiTroId;
                    db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Id = check.Id, JsonRequestBehavior.AllowGet });
            }
        }
        public static List<ValidateError> Validate(TaiKhoan user)
        {
            var errors = new List<ValidateError>();
            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Username.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Username",
                    Description = "Username không được trống."
                });
            }

            if (String.IsNullOrEmpty(user.Password.ToString()) || String.IsNullOrEmpty(user.Password.ToString().Trim()))
            {

                errors.Add(new ValidateError
                {
                    Id = "Password",
                    Description = "Password không được trống."
                });
            }
            if (user.VaiTroId <= 0)
            {

                errors.Add(new ValidateError
                {
                    Id = "VaiTroId",
                    Description = "Vai trò không được trống."
                });
            }
            return errors;
        }
        [HttpPost]
        [IsAuthorize (MenuKey = "TaiKhoan")]
        public ActionResult GetItem(int Id)
        {
            var check = db.TaiKhoans.FirstOrDefault(n => n.Id == Id);
            var data = JsonConvert.SerializeObject(check, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [IsAuthorize (MenuKey = "TaiKhoan")]
        public ActionResult XoaItem(int Id)
        {
            var check = db.TaiKhoans.FirstOrDefault(n => n.Id == Id);
            if (check != null)
            {
                db.TaiKhoans.Remove(check);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}