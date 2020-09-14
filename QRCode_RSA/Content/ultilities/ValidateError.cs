using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRCode_RSA.Content.ultilities
{
    public class ValidateError
    {
        public string Id { get; set; }

        public string Name => "Error"+ Id;

        public string Description { get; set; }
    }
}