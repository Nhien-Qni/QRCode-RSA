using QRCode_RSA.Content.ultilities;
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
        public ActionResult Index()
        {
            ConvertWordtoImage();
            return View();
        }
        [HttpPost]
        public JsonResult QuetMa(string key)
        {
            try
            {
                string data = "";
                if (rsa.PrivateKeyXML == null)
                {
                    return Json(new { isError = "Chưa có khóa bí mật" }, JsonRequestBehavior.AllowGet);
                }
                string[] separator = { ",$$$$$" };
                var splitkey = key.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                //string dulieuLayDuoc = Common.FromHexString(key);
                int id = int.Parse(splitkey[0]);
                var check = db.Users.FirstOrDefault(n => n.Id == id);
                if ( check != null)
                {
                    string HoTen = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[1])); 
                    DateTime? NgaySinh = DateTime.Parse(splitkey[2]);
                    string NoiCuTru = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[3]));
                    string QuocGia = Encoding.UTF8.GetString(Convert.FromBase64String(splitkey[4]));
                    if (check.HoTen != HoTen && check.NgaySinh != NgaySinh && check.NoiCuTru != NoiCuTru && check.QuocGia != QuocGia)
                    {
                        return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
                    }
                    data =check.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(check.HoTen)) + ",$$$$$ " + (check.NgaySinh != null ? check.NgaySinh.ToString() : "") + ",$$$$$ " + (check.SoHieu != null ? check.SoHieu.ToString() : "") + ",$$$$$ " + (check.SoBangCap != null ? check.SoBangCap.ToString() : "");
                }
                else
                {
                    return Json(new { isError = "QRcode không chính xác" }, JsonRequestBehavior.AllowGet);
                }
                var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, splitkey[5]);
                if (!string.IsNullOrEmpty(bangroD) && !string.IsNullOrEmpty(data))
                {
                    if (bangroD.Equals(Encoding.Unicode.GetString(Common.HashString(data))))
                    {
                        var duLieu = _iMapperView.Map<User, UserViewModel>(check);
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
        public void ConvertWordtoImage()
        {
            string filename1 = "Doc1.docx";
            string startupPath = @"C:\Users\NHIEN\Desktop\";
            Microsoft.Office.Interop.Word.Application myWordApp = new Microsoft.Office.Interop.Word.Application();
            Document myWordDoc = new Document();
            object missing = System.Type.Missing;
            object path1 = startupPath + filename1;
            myWordDoc = myWordApp.Documents.Add(path1, missing, missing, missing);

            foreach (Microsoft.Office.Interop.Word.Window window in myWordDoc.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var bits = pane.Pages[i].EnhMetaFileBits;
                        var target = path1 + "_image.doc";
                        try
                        {
                            using (var ms = new MemoryStream((byte[])(bits)))
                            {
                                var image = System.Drawing.Image.FromStream(ms);
                                image = new Bitmap(image);
                                var pngTarget = Path.ChangeExtension(target, "png");
                                image.Save(pngTarget, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        catch (System.Exception ex)
                        { }
                    }
                }
            }
            myWordDoc.Close(Type.Missing, Type.Missing, Type.Missing);
            myWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
        }
    }
}