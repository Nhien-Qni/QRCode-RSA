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

namespace QRCode_RSA.Controllers
{
    public class TaoMaController : Controller
    {
       
        Tool.TaoMa rsa = new Tool.TaoMa();
        // GET: TaoMa
        public ActionResult Index()
        {
            //cc
            ////B1 : Trích thông tin và tạo mã từ khóa bí mật
            ////Tạo PublicKey, PrivateKey
            rsa.AssignNewKey("PassW0rd@123");
            ////B2 : Băm nội dung thông tin
            byte[] duLieuBam = HashString("thong tin can ma hoa");
            ////B3: Mã hóa dữ liệu băm => Chữ ký số
            var duLieuMaHoa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, duLieuBam);
            ////B4: Tạo QrCode từ dữ liệu đã mã hóa
            string TaoQR = ToHexString(duLieuMaHoa);
            TaoQRCode(TaoQR);

            ////B5: Quét lấy dữ liệu
            string dulieuLayDuoc = FromHexString(TaoQR);
            ////B6: Giải mã dữ liệu
            var bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, dulieuLayDuoc);
            // B7 : Băm thông tin giải mã được 
            //string duLieuDaGiaiMa = HashString("thông tin cần mã hóa");
            ////B7 So sánh bangroD vs duLieuMaHoa
            if (bangroD == null)
            //    return null;
            return View();
            return View();
        }
        [HttpPost]
        public ActionResult TaoMa(string key)
        {
            //MaHoa
            //string bangMa = rsa.Encrypt_string(rsa.PublicOnlyKeyXML, "Nội dung mã hóa");

            //GiaiMa
            //string bangroD = rsa.Decrypt_string(rsa.PrivateKeyXML, bangMa);
            return View();
        }
        public static byte[] HashString(string data)
        {
            //Declarations
            var sha = SHA256.Create();

            var originalBytes = Encoding.Default.GetBytes(CreateSalt(data));

            return  sha.ComputeHash(originalBytes);
        }
        public void TaoQRCode(string data)
        {
            //Tạo QRCode
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Jpeg);
                    string filePath = Path.Combine(Server.MapPath("/images"), DateTime.UtcNow.ToBinary() + ".jpg");
                    System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                }
            }
        }
        private static string CreateSalt(string id)
        {
            var userBytes = Encoding.ASCII.GetBytes(id);
            long xored = 0x00;

            foreach (var x in userBytes)
            {
                xored = ++xored ^ x;
            }

            var rand = new Random(Convert.ToInt32(xored));
            var salt = rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            salt += rand.Next().ToString(CultureInfo.InvariantCulture);
            return salt.Substring(salt.Length / 5, salt.Length / 2);
        }
        public static string ToHex(byte[] value)
        {
            var stringBuilder = new StringBuilder();
            if (value != null)
            {
                var temp = string.Empty;
                foreach (var b in value)
                {
                    temp += HexStringTable[b];
                    if (temp.Length == 8)
                    {
                        stringBuilder.Append(temp + "-");
                        temp = string.Empty;
                    }
                }
            }

            return stringBuilder.Replace("-", "", stringBuilder.Length - 1, 1).ToString();
        }
        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }
        private static readonly string[] HexStringTable =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };
    }
}