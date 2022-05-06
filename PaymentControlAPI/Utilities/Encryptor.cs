using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PaymentControlAPI.Utilities
{
    public class Encryptor
    {
       static Random _random = new Random();
        static IConfiguration _config;
        public Encryptor(IConfiguration config)
        {
            _config = config;

        }
        
        public static string SHA256Hash(string input)
        {
            using (SHA256 hasher = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = hasher.ComputeHash(Encoding.Unicode.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    //sBuilder.Append(data[i].ToString("X2"));
                    sBuilder.Append(data[i].ToString("X2"));
                }
                //return sBuilder.ToString();                
                return sBuilder.ToString().ToLower();
            }
        }

        internal static string myhmac512Hash(string value)
        {
            string key = AppSettings.Instance.Get<string>("HMAC512Key");

            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

            var valueBytes = System.Text.Encoding.UTF8.GetBytes(value);
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

            var alg = new System.Security.Cryptography.HMACSHA512(keyBytes);
            var hash = alg.ComputeHash(valueBytes);

            var result = asHex(hash);

            return result;
        }

        private static string asHex(byte[] buf)
        {
            var strBuilder = new StringBuilder(buf.Length * 2);
            int i = 0;
            while (i < buf.Length)
            {
                if ((buf[i] & 255) < 16)
                {
                    strBuilder.Append("0");
                }

                strBuilder.Append(Convert.ToString(buf[i] & 255, 16));
                i = i + 1;
            }

            return strBuilder.ToString();
        }

       


    }
}
