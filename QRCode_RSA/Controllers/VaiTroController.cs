using QRCode_RSA.Common;
using Newtonsoft.Json;
using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using QRCode_RSA.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class VaiTroController : Controller
    {
        QRCodeEntities db;
        public VaiTroController()
        {
            db = new QRCodeEntities();
        }
        // GET: TaiKhoan
        [IsAuthorize(MenuKey = "VaiTro")]
        public ActionResult Index()
        {
            VaiTroViewModel mymodel = new VaiTroViewModel();
            mymodel.VaiTro = db.VaiTroes.ToList();
            mymodel.ListMenu = db.Menus.ToList();
            return View(mymodel);
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "VaiTro")]
        public ActionResult ThemSua(VaiTro vaitro)
        {
            int id = vaitro.Id;
            var errors = Validate(vaitro);
            if (errors.Count > 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }
            if (id == 0)
            {
                try
                {
                    db.VaiTroes.Add(vaitro);
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
                var check = db.VaiTroes.AsNoTracking().FirstOrDefault(n => n.Id == vaitro.Id);
                if (check != null)
                {
                    check.MenuId = vaitro.MenuId;
                    check.Ten = vaitro.Ten;
                    if (check.PhanQuyens.Count > 0)
                    {
                        foreach (var pd in check.PhanQuyens.ToList())
                        {
                            db.Entry(pd).State = EntityState.Deleted;
                        }
                    }
                    foreach (var item in vaitro.PhanQuyens)
                    {
                        db.PhanQuyens.Add(item);
                    }
                    db.Entry(check).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Id = check.Id, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "VaiTro")]
        public ActionResult GetItem(int Id)
        {
            var check = db.VaiTroes.FirstOrDefault(n => n.Id == Id);
            var data = JsonConvert.SerializeObject(check, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "VaiTro")]
        public ActionResult XoaItem(int Id)
        {
            var check = db.VaiTroes.FirstOrDefault(n => n.Id == Id);
            if (check != null)
            {
                if (db.PhanQuyens.FirstOrDefault(n => n.VaiTroId == check.Id) == null && db.TaiKhoans.FirstOrDefault(n=> n.VaiTroId == check.Id) == null)
                {
                    db.VaiTroes.Remove(check);
                    db.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public List<string> VaiTroPhanQuyenStr(int vaitroId)
        {
            using (var db = new QRCodeEntities())
            {
                var data = (from entity in db.PhanQuyens
                            where entity.VaiTroId == vaitroId
                            select entity.Menu.Ten
                            ).ToList();

                return data;
            }
        }
        public static List<ValidateError> Validate(VaiTro vaitro)
        {
            var errors = new List<ValidateError>();
            if (String.IsNullOrEmpty(vaitro.Ten) || String.IsNullOrEmpty(vaitro.Ten.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "Ten",
                    Description = "Tên vai trò không được trống."
                });
            }
            if (vaitro.MenuId <= 0)
            {

                errors.Add(new ValidateError
                {
                    Id = "MenuId",
                    Description = "Cần chọn menu chính."
                });
            }
            return errors;
        }
    }
}