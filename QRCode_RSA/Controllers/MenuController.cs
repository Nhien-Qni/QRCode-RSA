using Dashboard.Common;
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
        // GET: Menu

        public MenuController()
        {
            _menuRepository = new MenuRepository();
        }

        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult Index()
        {
            return View();
        }

        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult GetMenu(int Id)
        {
            var check = db.Menus.FirstOrDefault(n => n.Id == Id);
            var data = _iMapperView.Map<User, TaiKhoanViewModel>(check);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult Them(MenuViewModel menuViewModel)
        {
            var data = _menuRepository.Them(menuViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult CapNhat(MenuViewModel menuViewModel)
        {
            var data = _menuRepository.CapNhat(menuViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [IsAuthorize(MenuKey = "Menu")]
        public ActionResult Xoa(int Id)
        {
            var data = _menuRepository.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MenuLookup()
        {
            var data = _menuRepository.MenuLookup();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}