﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QRCode_RSA.Content.ultilities
{
    public  class Common
    {

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
        public static byte[] HashString(string data)
        {
            //Declarations
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //var sha = SHA256.Create();

            var originalBytes = Encoding.Default.GetBytes(CreateSalt(data));

            return hashmd5.ComputeHash(originalBytes);
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
    }
}