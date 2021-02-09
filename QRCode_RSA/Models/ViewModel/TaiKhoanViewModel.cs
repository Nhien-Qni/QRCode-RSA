using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCode_RSA.Models.ViewModel
{
    public class TaiKhoanViewModel : TaiKhoan
    {
        public string Code { get; set; }
        public string CurentCode { get; set; }
    }
}