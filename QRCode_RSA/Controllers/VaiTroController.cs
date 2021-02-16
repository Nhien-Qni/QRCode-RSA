using Dashboard.Common;
using Newtonsoft.Json;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
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
            return View(db.VaiTroes.ToList());
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "VaiTro")]
        public ActionResult ThemSua(VaiTro vaitro)
        {
            int id = vaitro.Id;
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
                    db.Entry(check).State = System.Data.Entity.EntityState.Modified;
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
                db.VaiTroes.Remove(check);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
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
    }
}