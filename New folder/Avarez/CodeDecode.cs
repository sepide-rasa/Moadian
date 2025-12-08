using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Avarez
{
    public class CodeDecode
    {
        public static string stringDecode(string a)
        {
            int l = a.Length, p;
            char[] k = new char[l];
            string s = "";
            for (int i = 0; i < l; i++)
            {
                p = (Convert.ToInt32(a[i]) - 1071) / 3;
                k[i] = Convert.ToChar(p);
                s = s + k[i].ToString();

            }
            return s;
        }
        public static string stringcode(string a)
        {
            int l = a.Length, p;
            char[] k = new char[l];
            string s = "";
            for (int i = 0; i < l; i++)
            {
                p = 3 * Convert.ToInt32(a[i]) + 1071;
                k[i] = Convert.ToChar(p);
                s = s + k[i].ToString();
            }

            return s;
        }
        public static string GenerateHash(string value)
        {
            SHA1 sha1 = SHA1.Create();
            //convert the input text to array of bytes
            if (value == null)
            {
                return "رمز عبور ضروری است.";
            }
            byte[] hashData = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString("X2"));
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
    }
}