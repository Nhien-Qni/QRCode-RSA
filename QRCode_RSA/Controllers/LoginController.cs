using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QRCode_RSA.Controllers
{
    public class LoginController : Controller
    {
        QRCodeEntities db;
        // GET: Login
        public ActionResult Index()
        {
            if (Session["user"] != null)
                return RedirectToAction("Index", "RSA");
            return View();
        }
        [HttpPost]
        public ActionResult Index(UsersViewModel userViewModel)
        {
            var errorStr = "";
            var isError = false;

            if (string.IsNullOrEmpty(userViewModel.Username))
            {
                errorStr += "<li>Tên truy cập không được trống</li>";
                isError = true;
            }

            if (string.IsNullOrEmpty(userViewModel.Password))
            {
                errorStr += "<li>Mật khẩu không được trống</li>";
                isError = true;
            }

            if (string.IsNullOrEmpty(userViewModel.Code))
            {
                errorStr += "<li>Mã xác thực không được trống</li>";
                isError = true;
            }
            else if (Regex.Replace(userViewModel.Code.Trim(), @"\s+", "")
                    != Regex.Replace(userViewModel.CurentCode.Trim(), @"\s+", ""))
            {
                errorStr += "<ol>Mã xác thực không đúng<ol>";
                isError = true;
            }

            if (isError)
            {
                ViewBag.ErrorMessage = $"<ol>{errorStr}</ol>";
                return View(userViewModel);
            }
            db = new QRCodeEntities();
            var user = db.Users.FirstOrDefault(n => n.Username == userViewModel.Username.Trim() && n.Password == userViewModel.Password);
            if (user != null && user.Username != null)
            {
                Session["User"] = !string.IsNullOrEmpty(user.Username) ? user.Username : "";
                return RedirectToAction("Index", "RSA");
            }

            ViewBag.ErrorMessage = $"<ol><li>Tên truy cập hoặc mật khẩu không đúng</li></ol>";

            return View(userViewModel);
        }
        public ActionResult LogOut()
        {
            Session.Remove("user");
            return RedirectToAction("Index", "Login");
        }
    }
}