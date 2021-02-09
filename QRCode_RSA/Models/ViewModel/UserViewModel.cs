using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCode_RSA.Models.ViewModel
{
    public class UserViewModel : User
    {
        public string NgaySinhString => NgaySinh != null ? NgaySinh.ToString("dd/MM/yyyy") : "";
        public HttpPostedFileBase AvatarFile { get; set; }
        public string savefilename { get; set; }
    }
}