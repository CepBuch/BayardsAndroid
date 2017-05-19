using Java.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bayards_Android
{
    static class CreditnailsProvider
    {

        public static string ConvertToMD5(string password)
        {

            MessageDigest digest = MessageDigest.GetInstance("MD5");
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            digest.Update(bytes);
            byte[] messageDigest = digest.Digest();

            StringBuilder hexString = new StringBuilder();

            foreach (var aMessageDigest in messageDigest)
            {
                string hexValue = aMessageDigest.ToString("X");

                while (hexValue.Length < 2)
                    hexValue = "0" + hexValue;

                hexString.Append(hexValue);
            }
            return hexString.ToString();
        }




    }
}