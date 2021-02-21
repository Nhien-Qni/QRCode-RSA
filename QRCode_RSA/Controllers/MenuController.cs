using QRCode_RSA.Common;
using Newtonsoft.Json;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class MenuController : Controller
    {
        QRCodeEntities db;
        public MenuController()
        {
            db = new QRCodeEntities();
        }
        // GET: TaiKhoan
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult Index()
        {
            return View(db.Menus.ToList());
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult ThemSua(Menu menu)
        {
            int id = menu.Id;
            if (id == 0)
            {
                try
                {
                    db.Menus.Add(menu);
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
                var check = db.Menus.AsNoTracking().FirstOrDefault(n => n.Id == menu.Id);
                if (check != null)
                {
                    check.MoTa = menu.MoTa;
                    check.Ten = menu.Ten;
                    db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Id = check.Id, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult GetItem(int Id)
        {
            var check = db.Menus.FirstOrDefault(n => n.Id == Id);
            var data = JsonConvert.SerializeObject(check, Formatting.Indented,
                           new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult XoaItem(int Id)
        {
            var check = db.Menus.FirstOrDefault(n => n.Id == Id);
            if (check != null)
            {
                if (db.PhanQuyens.FirstOrDefault(n => n.MenuId == check.Id) == null && db.VaiTroes.FirstOrDefault(n => n.MenuId == check.Id) == null)
                {
                    db.Menus.Remove(check);
                    db.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}