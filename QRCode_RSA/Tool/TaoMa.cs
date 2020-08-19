using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace QRCode_RSA.Tool
{
    public class TaoMa
    {
        RSACryptoServiceProvider rsa = null;
        public string PrivateKeyXML;
        public string PublicOnlyKeyXML;
        public void AssignNewKey(string keymaster)
        {
            const int PROVIDER_RSA_FULL = 1;
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = keymaster;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            rsa = new RSACryptoServiceProvider(1024,cspParams);

            //Pair of public and private key as XML string.
            //Do not share this to other party
            PublicOnlyKeyXML = rsa.ToXmlString(false);

            //Private key in xml file, this string should be share to other parties
            PrivateKeyXML = rsa.ToXmlString(true);

        }
        public string Encrypt_string(string publicKeyXML, byte[] dataToDycript)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(publicKeyXML);
                var test = rsa.Encrypt(dataToDycript, true);
                string EncryptedResult = System.Convert.ToBase64String(test);
                return EncryptedResult;
            }
            catch (Exception e)
            {
                return "Mã hóa thất bại" + e.Message;
            }
        }
        public string Decrypt_string(string publicPrivateKeyXML, string encryptedData)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(publicPrivateKeyXML);
                byte[] toDecryptData = System.Convert.FromBase64String(encryptedData);
                return System.Text.Encoding.Unicode.GetString(rsa.Decrypt(toDecryptData, true));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}