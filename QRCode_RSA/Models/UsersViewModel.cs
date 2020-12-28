using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCode_RSA.Models
{
    public class UsersViewModel : User
    {
        public string Code { get; set; }
        public string CurentCode { get; set; }
        public HttpPostedFileBase AvatarFile { get; set; }
    }
}