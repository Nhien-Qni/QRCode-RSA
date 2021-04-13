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
using QRCode_RSA.Models.ViewModel;
using AutoMapper;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using QRCode_RSA.Common;

namespace QRCode_RSA.Controllers
{
    public class TaoMaController : Controller
    {
        QRCodeEntities db;
        public Tool.TaoMa rsa;
        private readonly IMapper _iMapperView = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserViewModel>();
        }).CreateMapper();
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserViewModel, User>();
        }).CreateMapper();
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
        [IsAuthorize(MenuKey = "QR")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "QR")]
        public ActionResult TaoQR(int id)
        {
            try
            {
                if (rsa.PublicOnlyKeyXML == null)
                {
                    return Json(new { isError = "Chưa tạo public key" }, JsonRequestBehavior.AllowGet);
                }
                var user = db.Users.FirstOrDefault(n => n.Id == id);
                string savefilename = DateTime.Now.ToFileTime().ToString();
                var duLieu = _iMapperView.Map<User, UserViewModel>(user);
                var data = user.Id + ",$$$$$ " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.HoTen)) + ",$$$$$ " + (user.NgaySinh != null ? user.NgaySinh.ToString() : "") + ",$$$$$ " + user.SoHieu.ToString() + ",$$$$$ " + (user.SoBangCap != null ? user.SoBangCap.ToString() : "") + ",$$$$$ " + savefilename;
                // Tạo PublicKey, PrivateKey
                byte[] duLieuBam = QRCode_RSA.Content.ultilities.Common.HashString(data);
                //var t = Convert.ToBase64String(duLieuBam);
                var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
                if (duLieuMaHoa.Contains("Mã hóa thất bại"))
                {
                    return Json(new { isError = duLieuMaHoa }, JsonRequestBehavior.AllowGet);
                }
                string TaoQR = data + ",$$$$$ " + duLieuMaHoa;
                //string TaoQR = Common.FromHexString(duLieuMaHoa);
                // Tạo QR Image
                var linkQRImage = QRCode_RSA.Content.ultilities.Common.TaoQRCode(TaoQR);
                var file = ConvertWordtoImage(duLieu, linkQRImage, savefilename);
                file.txtMaQuet = TaoQR;
                System.IO.File.Delete(linkQRImage);
                return new JsonResult()
                {
                    Data = file,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
                //return Json(ConvertWordtoImage(duLieu, linkQRImage), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = "Tạo thất bại: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "QR")]
        public ActionResult ThemSua(UserViewModel user)
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
                try
                {
                    var data = _iMapper.Map<UserViewModel, User>(user);
                    db.Users.Add(data);
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
                var check = db.Users.AsNoTracking().FirstOrDefault(n => n.Id == user.Id);
                if (check != null)
                {
                    check = _iMapper.Map<UserViewModel, User>(user);
                    db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Id = check.Id, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "QR")]
        public ActionResult GetItem(int Id)
        {
            var check = db.Users.FirstOrDefault(n => n.Id == Id);
            var data = _iMapperView.Map<User, UserViewModel>(check);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [IsAuthorize(MenuKey = "QR")]
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

            if (String.IsNullOrEmpty(user.SoHieu.ToString()) || String.IsNullOrEmpty(user.SoHieu.ToString().Trim()))
            {

                errors.Add(new ValidateError
                {
                    Id = "SoHieu",
                    Description = "Số hiệu không được trống."
                });
                int tempInt = 0;
                bool result = int.TryParse(user.SoHieu.ToString(), out tempInt);
                if (!result)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "SoHieu",
                        Description = "Số hiệu phải là số."
                    });
                }

            }
            if (String.IsNullOrEmpty(user.SoBangCap) || String.IsNullOrEmpty(user.SoBangCap.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "SoBangCap",
                    Description = "Số bằng cấp không được trống."
                });
            }
            if (String.IsNullOrEmpty(user.XepLoai) || String.IsNullOrEmpty(user.XepLoai.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "XepLoai",
                    Description = "Xếp loại không được trống."
                });
            }
            if (String.IsNullOrEmpty(user.HinhThuc) || String.IsNullOrEmpty(user.HinhThuc.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "HinhThuc",
                    Description = "Hình thức không được trống."
                });
            }
            if (String.IsNullOrEmpty(user.NganhDaoTao) || String.IsNullOrEmpty(user.NganhDaoTao.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "NganhDaoTao",
                    Description = "Ngành đào tạo không được trống."
                });
            }
            //if (String.IsNullOrEmpty(user.NoiCuTru) || String.IsNullOrEmpty(user.NoiCuTru.Trim()))
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "NoiCuTru",
            //        Description = "Nơi cư trú không được trống."
            //    });
            //}
            //if (String.IsNullOrEmpty(user.QuocGia) || String.IsNullOrEmpty(user.QuocGia.Trim()))
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "QuocGia",
            //        Description = "Quốc gia không được trống."
            //    });
            //}
            if (String.IsNullOrEmpty(user.NgaySinh.ToString()) || user.NgaySinh.Year == 1)
            {
                errors.Add(new ValidateError
                {
                    Id = "NgaySinh",
                    Description = "Ngày sinh không được trống."
                });
            }
            return errors;
        }
        public static FileResultViewModel ConvertWordtoImage(UserViewModel check, string linkQRImage, string savefilename)
        {
            string filename1 = "Doc1.docx";
            const string saveFolder = "/Storage";
            const string saveImage = "images";
            string startupPath = System.Web.HttpContext.Current.Server.MapPath("/Storage");
            Microsoft.Office.Interop.Word.Application myWordApp = new Microsoft.Office.Interop.Word.Application();
            //Document myWordDoc = new Document();
            object missing = System.Type.Missing;
            object path1 = Path.Combine(startupPath, filename1);
            var myWordDoc = myWordApp.Documents.Add(path1, missing, missing, missing);
            foreach (Bookmark bm in myWordDoc.Bookmarks)
            {
                switch (bm.Name)
                {
                    case "Ten":
                        bm.Range.Text = check.HoTen.Trim();
                        break;

                    case "GioiTinh":
                        bm.Range.Text = check.GioiTinh == true ? "Nam" : "Nữ";
                        break;

                    case "HinhThuc":
                        bm.Range.Text = check.HinhThuc.Trim(); ;
                        break;

                    case "NganhDaoTao":
                        bm.Range.Text = check.NganhDaoTao.Trim();
                        break;

                    case "NgaySinh":
                        bm.Range.Text = check.NgaySinhString.Trim();
                        break;

                    case "SoHieu":
                        bm.Range.Text = check.SoHieu.ToString();
                        break;
                    case "SoBangCap":
                        bm.Range.Text = check.SoBangCap.Trim();
                        break;
                    case "XepLoai":
                        bm.Range.Text = check.XepLoai.Trim();
                        break;
                    case "QRCode":
                        //var sparePicture = ByteArrayToImage(QRCodeByte);
                        object oRange = bm.Range;
                        //var link_anh = System.Web.HttpContext.Current.Server.MapPath("/images") + "\\no-image.jpg";
                        /*Microsoft.Office.Interop.Word.InlineShape ils  =*/
                        myWordDoc.InlineShapes.AddPicture(linkQRImage, ref missing, ref missing, ref oRange);
                        // ils.Height = 2339 ;
                        //ils.Width = 1654;
                        break;
                    case "NgayHienTai":
                        bm.Range.Text = DateTime.Now.Day.ToString();
                        break;
                    case "ThangHienTai":
                        bm.Range.Text = DateTime.Now.Month.ToString();
                        break;
                    case "NamHienTai":
                        bm.Range.Text = DateTime.Now.Year.ToString();
                        break;
                }
            }
            var target = Path.Combine(startupPath, saveImage);
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            foreach (Microsoft.Office.Interop.Word.Window window in myWordDoc.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var bits = pane.Pages[1].EnhMetaFileBits;
                        try
                        {
                            using (var ms = new MemoryStream((byte[])(bits)))
                            {
                                byte[] resizedImage;
                                var image = System.Drawing.Image.FromStream(ms);
                                image = new Bitmap(image, new System.Drawing.Size(1654, 2339));
                                image = Transparent2Color(image, Color.White);
                                var pngTarget = Path.ChangeExtension(target + "\\" + savefilename, "png");
                                image.Save(pngTarget, ImageFormat.Png);
                                using (MemoryStream streamResized = new MemoryStream())
                                {
                                    image.Save(streamResized, ImageFormat.Png);
                                    resizedImage = streamResized.ToArray();
                                }
                                var fileVm = new FileResultViewModel();
                                fileVm.Content = Convert.ToBase64String(resizedImage);
                                fileVm.FileName = savefilename + ".png";
                                myWordDoc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
                                myWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
                                Marshal.ReleaseComObject(myWordDoc);
                                Marshal.ReleaseComObject(myWordApp);
                                return fileVm;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            myWordDoc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
                            myWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
                            Marshal.ReleaseComObject(myWordDoc);
                            Marshal.ReleaseComObject(myWordApp);
                            return null;
                        }
                    }
                }
            }
            //var fileNameTemp = $"ImportDV_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            //var filePathTemp = Path.Combine(fullPath, fileNameTemp);
            //file.SaveAs(filePathTemp);
            //myWordDoc.Close(WdSaveOptions.wdDoNotSaveChanges);
            return null;
        }
        public static Image Transparent2Color(Image bmp1, Color target)
        {
            Image bmp2 = new Bitmap(bmp1.Width, bmp1.Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp1.Size);
            using (Graphics G = Graphics.FromImage(bmp2))
            {
                G.Clear(target);
                G.DrawImageUnscaledAndClipped(bmp1, rect);
            }
            return bmp2;
        }
    }
}