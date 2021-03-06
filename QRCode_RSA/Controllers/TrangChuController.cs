﻿using QRCode_RSA.Content.ultilities;
using QRCode_RSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using AutoMapper;
using QRCode_RSA.Models.ViewModel;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Office.Interop.Word;
using System.Drawing;
using System.Runtime.InteropServices;
using QRCode_RSA.Common;

namespace QRCode_RSA.Controllers
{
    public class TrangChuController : Controller
    {
        QRCodeEntities db;
        private readonly IMapper _iMapperView = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserViewModel>();
        }).CreateMapper();
        Tool.TaoMa rsa = new Tool.TaoMa();
        public TrangChuController()
        {
            db = new QRCodeEntities();
            rsa = new Tool.TaoMa();
            if (db.RSAs.FirstOrDefault() != null)
            {
                rsa.PrivateKeyXML = db.RSAs.FirstOrDefault().PrivateKey;
                rsa.PublicOnlyKeyXML = db.RSAs.FirstOrDefault().PublicKey;
            }

        }
        // GET: TrangChu
        [IsAuthorize(MenuKey = "TrangChu")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "TrangChu")]
        public JsonResult QuetMa(string key)
        {
            try
            {
                string data = "";
                string savefilename = "";
                if (rsa.PrivateKeyXML == null)
                {
                    return Json(new { isError = "Chưa có khóa bí mật" }, JsonRequestBehavior.AllowGet);
                }
                string[] separator = { ",$$$$$" };
                var splitkey = key.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                //string dulieuLayDuoc = Common.FromHexString(key);
                int id = int.Parse(splitkey[0]);
                var check = db.Users.FirstOrDefault(n => n.Id == id);
                if (check != null)
                {
                    string HoTen = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[1]));
                    DateTime? NgaySinh = DateTime.Parse(splitkey[2]);
                    string SoHieu = splitkey[3];
                    string SoBangCap = splitkey[4];
                    savefilename = splitkey[5].Trim();
                    if (check.HoTen != HoTen && check.NgaySinh != NgaySinh && check.SoHieu.ToString() != SoHieu && check.SoBangCap != SoBangCap)
                    {
                        return Json(new { isError = "Thông tin sinh viên không chính xác" }, JsonRequestBehavior.AllowGet);
                    }
                    data = check.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(check.HoTen)) + ",$$$$$ " + (check.NgaySinh != null ? check.NgaySinh.ToString() : "") + ",$$$$$ " + check.SoHieu.ToString() + ",$$$$$ " + (check.SoBangCap != null ? check.SoBangCap.ToString() : "") + ",$$$$$ " + savefilename;
                }
                else
                {
                    return Json(new { isError = "Không có thông tin sinh viên." }, JsonRequestBehavior.AllowGet);
                }
                var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, splitkey[6]);
                if (!string.IsNullOrEmpty(bangroD) && !string.IsNullOrEmpty(data))
                {
                    if (bangroD.Equals(Encoding.Unicode.GetString(QRCode_RSA.Content.ultilities.Common.HashString(data))))
                    {
                        var duLieu = _iMapperView.Map<User, UserViewModel>(check);
                        duLieu.savefilename = "/Storage/images/" + savefilename + ".png";
                        //ConvertWordtoImage(duLieu);
                        return Json(new { isSuccess = "Qrcode chính xác", DuLieu = duLieu }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}